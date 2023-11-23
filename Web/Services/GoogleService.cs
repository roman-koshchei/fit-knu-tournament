using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;

namespace Web.Services;

public class GoogleService
{
    private readonly SignInManager<User> signInManager;
    private readonly UserManager<User> userManager;
    private readonly Jwt jwt;

    public GoogleService(SignInManager<User> signInManager, UserManager<User> userManager, Jwt jwt)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.jwt = jwt;
    }

    /// <summary>
    /// Sign in user with external auth
    /// </summary>
    /// <returns>Token if success, otherwise null</returns>
    public async Task<string?> SignInUserWithExternal(User user, ExternalLoginInfo loginInfo)
    {
        var signInResult = await signInManager.ExternalLoginSignInAsync(
            loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true
        );
        if (!signInResult.Succeeded) return null;

        return jwt.Token(user.Id, user.Version);
    }

    /// <summary>
    /// Create new user for external auth and new user token.
    /// </summary>
    /// <returns>Token if success, otherwise null</returns>
    public async Task<string?> CreateUserWithExternal(string email, ExternalLoginInfo loginInfo)
    {
        var newUser = new User(email);

        var result = await userManager.CreateAsync(newUser);
        if (!result.Succeeded) return null;

        // Add the external login for the user
        result = await userManager.AddLoginAsync(newUser, loginInfo);
        if (!result.Succeeded) return null;

        return jwt.Token(newUser.Id, newUser.Version);
    }
}