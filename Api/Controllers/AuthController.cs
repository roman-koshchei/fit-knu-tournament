using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Add the Authorize attribute at the controller level to restrict access to authenticated users
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly Jwt jwt;

    public AuthController(UserManager<User> userManager, Jwt jwt)
    {
        _userManager = userManager;
        this.jwt = jwt;
    }

    // GET api/login ROSTIK
    // - Logging in
    [HttpPost("login")]
    [AllowAnonymous] // Allow anonymous access for login
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var token = jwt.Token(user.Id, user.Version);

        return Ok(new { Token = token });
    }

    // POST api/register BOHDAN
    // - Registration
    [HttpPost("register")]
    [AllowAnonymous] // Allow anonymous access for registration
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var existingUser = await _userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        var newUser = new User { UserName = model.UserName, Email = model.Email };
        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded)
        {
            var token = jwt.Token(newUser.Id, newUser.Version);

            return Ok(new { Token = token });
        }

        return BadRequest(result.Errors);
    }

    // PUT api/emailChange ROMA (Team lead big boss)
    // - Updating email
    [HttpPatch("updateEmail")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user id from the authenticated user's claims

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        // Check if the authenticated user is the owner of the account
        if (user.Id != userId)
        {
            return Forbid(); // Return 403 Forbidden if the user is not the owner
        }

        user.Email = model.NewEmail;
        user.NormalizedEmail = model.NewEmail.ToUpper();
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok("Email updated successfully");
        }

        return BadRequest(result.Errors);
    }

    // DELETE api/deleteUser ROMA KORINEVSKII
    [HttpDelete("deleteUser")]
    public async Task<IActionResult> Delete()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        if (user.Id != userId)
        {
            return Forbid();
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Ok("User deleted successfully");
        }

        return BadRequest(result.Errors);
    }
}

// Models for input data
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class RegisterModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UpdateEmailModel
{
    public string NewEmail { get; set; }
}