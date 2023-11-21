using Data;
using Microsoft.AspNetCore.Mvc;

/*
User model has such fields:
    Id: The unique identifier for the user.
    UserName: The user's username.
    NormalizedUserName: A normalized version of the username, used for case-insensitive comparisons and lookups.
    Email: The user's email address.
    NormalizedEmail: A normalized version of the email address, used for case-insensitive comparisons and lookups.
    EmailConfirmed: A flag indicating whether the email address has been confirmed by the user.
    PasswordHash: The hashed and salted password.
    SecurityStamp: A random value that should be regenerated when a user changes their password, email, or other critical information.
    ConcurrencyStamp: A value used to detect concurrent updates, which is typically updated whenever a user is modified.
    TwoFactorEnabled: A flag indicating whether two-factor authentication is enabled for the user.
    LockoutEnd: The date and time when the user's lockout period will end (if they are currently locked out).
    LockoutEnabled: A flag indicating whether lockout is enabled for the user.
    AccessFailedCount: The number of failed access attempts.
 */

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // GET api/login ROSTIK
    // - Logging in
    [HttpGet("login")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(id);
    }

    // POST api/register BOHDAN
    // - Registration
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] string value)
    {
        return Ok();
    }

    // DELETE api/deleteUser ROMA KORINEVSKII
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok();
    }
}