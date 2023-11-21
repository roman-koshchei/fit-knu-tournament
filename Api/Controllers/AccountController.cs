using Backend.Auth;
using Data;
using Data.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

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

    // id will be removed and taken from token
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

    // TODO: in 1 transaction
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
}