using Data;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace Web.Config;

/// <summary>
/// Seach for token in Header and Cookie.
/// Check version of User.
/// If API return 401, otherwise redirect to Home/Login page
/// </summary>
public class CustomAuthMiddleware
{
    private readonly RequestDelegate next;
    private readonly Db db;
    private readonly Jwt jwt;

    public CustomAuthMiddleware(RequestDelegate next, IServiceProvider provider)
    {
        this.next = next;

        var scope = provider.CreateScope();
        db = scope.ServiceProvider.GetRequiredService<Db>();
        jwt = scope.ServiceProvider.GetRequiredService<Jwt>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requiresAuthentication = RequiresAuthentication(context);
        if (!requiresAuthentication)
        {
            await next(context);
            return;
        }

        var isApi = context.Request.Path.ToString().StartsWith("/api");

        var uid = context.User.Uid();
        var versionParsed = int.TryParse(context.User.FindFirst(Jwt.Version)?.Value, out var version);
        if (!versionParsed)
        {
            Unauthorized(context, isApi);
            return;
        }

        var versionOk = await db.Users.Have(x => x.Id == uid && x.Version == version);
        if (!versionOk)
        {
            Unauthorized(context, isApi);
            return;
        }

        await next(context);
    }

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

    private static bool RequiresAuthentication(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null) return false;

        var authorize = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
        return authorize != null;
    }

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

public static class CustomAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomAuthMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomAuthMiddleware>();
    }
}