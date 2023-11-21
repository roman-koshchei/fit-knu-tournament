using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
