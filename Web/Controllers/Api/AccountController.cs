using Data;
using Data.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Config;

namespace Web.Controllers.Api;

/// <summary>
/// API controller for managing user account-related operations.
/// </summary>
[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly Db db;
    private readonly UserManager<User> userManager;

    public AccountController(Db db, UserManager<User> userManager)
    {
        this.db = db;
        this.userManager = userManager;
    }

    /// <summary>
    /// Represents the request body for updating user email.
    /// </summary>
    public record EmailBody(string Email);

    /// <summary>
    /// Updates the user's email.
    /// </summary>
    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> Email([FromBody] EmailBody body)
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return NotFound();

        // Update user email and version.
        user.Email = body.Email;
        user.NormalizedEmail = body.Email.ToUpper();
        user.Version += 1;

        var saved = await db.Save();
        return saved ? Ok() : Problem();
    }

    /// <summary>
    /// Represents the request body for updating user password.
    /// </summary>
    public record PasswordBody(string OldPassword, string NewPassword);

    /// <summary>
    /// Updates the user's password.
    /// </summary>
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> Password([FromBody] PasswordBody body)
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return NotFound();

        // Change user password using UserManager.
        var result = await userManager.ChangePasswordAsync(user, body.OldPassword, body.NewPassword);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // Update user version and save changes.
        user.Version += 1;
        var saved = await db.Save();

        return saved ? Ok() : Problem();
    }

    /// <summary>
    /// Deletes the user account.
    /// </summary>
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return NotFound();

        // Delete user using UserManager.
        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded) return Ok();

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Represents the response output for the "Me" endpoint.
    /// </summary>
    public record MeOutput(string Id, string? Email);

    /// <summary>
    /// Retrieves information about the authenticated user.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var uid = User.Uid();

        var user = await db.Users.Where(x => x.Id == uid)
            .Select(x => new MeOutput(x.Id, x.Email ?? ""))
            .QueryOne();

        if (user == null) return NotFound();
        return Ok(user);
    }
}