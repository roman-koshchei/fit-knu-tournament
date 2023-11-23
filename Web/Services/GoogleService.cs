using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Web.Services
{
    /// <summary>
    /// Service for handling Google authentication-related operations.
    /// </summary>
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
        /// Sign in user with external authentication.
        /// </summary>
        /// <returns>Token if success, otherwise null.</returns>
        public async Task<string?> SignInUserWithExternal(User user, ExternalLoginInfo loginInfo)
        {
            var signInResult = await signInManager.ExternalLoginSignInAsync(
                loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true
            );

            // Return null if sign-in is not successful.
            if (!signInResult.Succeeded) return null;

            // Generate a JWT token for the authenticated user.
            return jwt.Token(user.Id, user.Version);
        }

        /// <summary>
        /// Create a new user for external authentication and generate a new user token.
        /// </summary>
        /// <returns>Token if success, otherwise null.</returns>
        public async Task<string?> CreateUserWithExternal(string email, ExternalLoginInfo loginInfo)
        {
            // Create a new user with the provided email.
            var newUser = new User(email);

            // Attempt to create the user in the identity system.
            var result = await userManager.CreateAsync(newUser);

            // Return null if user creation is not successful.
            if (!result.Succeeded) return null;

            // Add the external login for the user.
            result = await userManager.AddLoginAsync(newUser, loginInfo);

            // Return null if adding external login is not successful.
            if (!result.Succeeded) return null;

            // Generate a JWT token for the newly created user.
            return jwt.Token(newUser.Id, newUser.Version);
        }
    }
}
