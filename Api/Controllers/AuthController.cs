using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


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
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // GET api/login ROSTIK
        // - Logging in
        [HttpGet("login")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/register BOHDAN
        // - Registration
        [HttpPost]
        public void Register([FromBody] string value)
        {
        }

        // PUT api/emailChange ROMA (Team lead big boss)
        // - Updating email
        [HttpPatch("{id}")]
        public void UpdateEmail(int id, [FromBody] string value)
        {
        }

        // PUT api/passwordChange
        // - Updating password
        [HttpPatch("{id}")]
        public void UpdatePassword(int id, [FromBody] string value)
        {
        }

        // DELETE api/deleteUser ROMA KORINEVSKII
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
