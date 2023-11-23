using Data;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace Web.Config;

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

        var token = GetToken(context.Request);
        if (token == null)
        {
        }

        //var versionParsed = int.TryParse(context.User.FindFirst(Jwt.Version)?.Value, out var version);
        //if (uid == null || !versionParsed)
        //{
        //    context.Response.StatusCode = 401;
        //    return;
        //}

        //var versionOk = await db.Users.Have(x => x.Id == uid && x.Version == version);
        //if (!versionOk)
        //{
        //    context.Response.StatusCode = 401;
        //    return;
        //}

        await next(context);
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