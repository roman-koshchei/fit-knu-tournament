using Data;
using Data.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Config;

namespace Web.Controllers.Api;

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

    public record EmailBody(string Email);

    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> Email([FromBody] EmailBody body)
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return NotFound();

        user.Email = body.Email;
        user.NormalizedEmail = body.Email.ToUpper();
        user.Version += 1;

        var saved = await db.Save();
        return saved ? Ok() : Problem();
    }

    public record PasswordBody(string OldPassword, string NewPassword);

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> Password([FromBody] PasswordBody body)
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return NotFound();

        var result = await userManager.ChangePasswordAsync(user, body.OldPassword, body.NewPassword);
        if (!result.Succeeded) return BadRequest(result.Errors);

        user.Version += 1;
        var saved = await db.Save();

        return saved ? Ok() : Problem();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return NotFound();

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded) return Ok();

        return BadRequest(result.Errors);
    }

    public record MeOutput(string Id, string? Email);

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