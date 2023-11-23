using Data;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace Web.Config;

/// <summary>
/// Middleware for custom authentication logic.
/// Searches for the token in the Header and Cookie, checks the version of the User.
/// If the API returns 401, otherwise redirects to the Home/Login page.
/// </summary>
public class CustomAuthMiddleware
{
    private readonly RequestDelegate next;
    private readonly Db db;
    private readonly Jwt jwt;

    public CustomAuthMiddleware(RequestDelegate next, IServiceProvider provider)
    {
        this.next = next;

        // Dependency injection of database and JWT services.
        var scope = provider.CreateScope();
        db = scope.ServiceProvider.GetRequiredService<Db>();
        jwt = scope.ServiceProvider.GetRequiredService<Jwt>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if authentication is required for the current context.
        var requiresAuthentication = RequiresAuthentication(context);
        if (!requiresAuthentication)
        {
            await next(context);
            return;
        }

        // Determine if the request is for an API.
        var isApi = context.Request.Path.ToString().StartsWith("/api");

        // Extract user ID and version from the context's user claims.
        var uid = context.User.Uid();
        var versionParsed = int.TryParse(context.User.FindFirst(Jwt.Version)?.Value, out var version);
        if (!versionParsed)
        {
            Unauthorized(context, isApi);
            return;
        }

        // Check if the user's version is valid.
        var versionOk = await db.Users.Have(x => x.Id == uid && x.Version == version);
        if (!versionOk)
        {
            Unauthorized(context, isApi);
            return;
        }

        // Continue processing the request.
        await next(context);
    }

    // Handles unauthorized access by setting the response status code or redirecting.
    private static void Unauthorized(HttpContext context, bool isApi)
    {
        if (isApi)
        {
            context.Response.StatusCode = 401;
        }
        else
        {
            context.Response.Redirect("/");
        }
    }

    // Checks if authentication is required for the current request endpoint.
    private static bool RequiresAuthentication(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null) return false;

        var authorize = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
        return authorize != null;
    }

    // Retrieves the token from the request headers or cookies.
    private static string? GetToken(HttpRequest request)
    {
        if (StringValues.IsNullOrEmpty(request.Headers.Authorization))
        {
            request.Cookies.TryGetValue("token", out var cookie);
            return cookie;
        }

        var header = request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer")) return null;

        var token = header.Substring("Bearer".Length).Trim();
        if (string.IsNullOrEmpty(token)) return null;

        return token;
    }
}

/// <summary>
/// Extension methods for integrating the CustomAuthMiddleware into the application's request processing pipeline.
/// </summary>
public static class CustomAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomAuthMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomAuthMiddleware>();
    }
}
