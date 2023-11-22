using Data;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Api;

[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly Db db;

    public TestController(Db db)
    {
        this.db = db;
    }

    [HttpGet("users")]
    public async Task<IActionResult> AllUsers()
    {
        return Ok(await db.Users.QueryMany());
    }
}