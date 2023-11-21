using Data;
using Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/[controller]")]
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

    // id will be removed and taken from token
    public record EmailBody(string Id, string Email);

    [HttpPut("email")]
    public async Task<IActionResult> Email([FromBody] EmailBody body)
    {
        var user = await db.Users.QueryOne(x => x.Id == body.Id);
        if (user == null) return NotFound();

        user.Email = body.Email;
        user.Version += 1;

        var saved = await db.Save();
        return saved ? Ok() : Problem();
    }

    public record PasswordBody(string Id, string OldPassword, string NewPassword);

    // PUT api/passwordChange
    // - Updating password
    [HttpPut("password")]
    public async Task<IActionResult> Password([FromBody] PasswordBody body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        if (user.Id != userId)
        {
            return Forbid();
        }

        var result = await userManager.ChangePasswordAsync(user, body.OldPassword, body.NewPassword);

        if (result.Succeeded)
        {
            return Ok("Password updated successfully");
        }

        return BadRequest(result.Errors);
    }
}