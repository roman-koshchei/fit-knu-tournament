using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly Db db;

    public AccountController(Db db)
    {
        this.db = db;
    }

    // id will be removed and taken from token
    public record EmailBody(string Id, string Email);

    [HttpPatch("email")]
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
    [HttpPatch("password")]
    public async Task<IActionResult> Password([FromBody] PasswordBody body)
    {
        return Ok();
    }
}