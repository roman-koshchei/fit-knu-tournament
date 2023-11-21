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
    public record EmailBody(string Id, string Email);

    [HttpPut("email")]
    public async Task<IActionResult> Email([FromBody] EmailBody body)
    {
        var user = await db.Users.QueryOne(x => x.Id == body.Id);
        if (user == null) return NotFound();

        user.Email = body.Email;
        user.NormalizedEmail = body.Email.ToUpper();
        user.Version += 1;

        var saved = await db.Save();
        return saved ? Ok() : Problem();
    }

    public record PasswordBody(string Id, string OldPassword, string NewPassword);

    [HttpPut("password")]
    public async Task<IActionResult> Password([FromBody] PasswordBody body)
    {
        var user = await db.Users.QueryOne(x => x.Id == body.Id);
        if (user == null) return NotFound();

        var result = await userManager.ChangePasswordAsync(user, body.OldPassword, body.NewPassword);
        if (result.Succeeded) return Ok();

        return BadRequest(result.Errors);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = "test";

        var user = await db.Users.QueryOne(x => x.Id == userId);
        if (user == null) return NotFound();

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded) return Ok();

        return BadRequest(result.Errors);
    }
}