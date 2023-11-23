using Lib;
using NuGet.Common;
using System.Security.Claims;

namespace Web.Config;

public static class AuthExtension
{
    /// <summary>
    /// Should be used only for [Authorize] routes.
    /// Otherwise throw error.
    /// </summary>
    /// <returns>Return user id from user claims principal.</returns>
    public static string Uid(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(Jwt.Uid)!.Value;
    }

    public static bool HaveUid(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(Jwt.Uid) != null;
    }

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