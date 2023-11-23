using Lib;
using System.Security.Claims;

namespace Web.Config;

/// <summary>
/// Provides extension methods for authentication-related functionality.
/// Should be used in conjunction with [Authorize] routes.
/// Otherwise, it may throw errors.
/// </summary>
public static class AuthExtension
{
    /// <summary>
    /// Retrieves the user ID from the claims principal.
    /// </summary>
    /// <returns>User ID extracted from the claims principal.</returns>
    public static string Uid(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(Jwt.Uid)!.Value;
    }

    public static string? MaybeUid(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(Jwt.Uid)?.Value;
    }

    /// <summary>
    /// Checks if the claims principal contains a user ID.
    /// </summary>
    /// <returns>True if the user ID is present; otherwise, false.</returns>
    public static bool HaveUid(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(Jwt.Uid) != null;
    }

    /// <summary>
    /// Adds an authentication cookie to the HTTP response.
    /// </summary>
    /// <param name="response">HTTP response to which the cookie will be added.</param>
    /// <param name="token">Authentication token to be stored in the cookie.</param>
    public static void AddAuthCookie(this HttpResponse response, string token)
    {
        response.Cookies.Append("token", token, new()
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTimeOffset.Now.AddDays(30)
        });
    }
}
