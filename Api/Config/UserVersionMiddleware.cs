using Backend.Auth;
using Data;
using Lib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Config;

public class UserVersionMiddleware
{
    private readonly RequestDelegate next;
    private readonly Db db;

    public UserVersionMiddleware(RequestDelegate next, IServiceProvider provider)
    {
        this.next = next;

        var scope = provider.CreateScope();
        db = scope.ServiceProvider.GetRequiredService<Db>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requiresAuthentication = RequiresAuthentication(context);

        if (!requiresAuthentication)
        {
            await next(context);
            return;
        }

        var uid = context.User.Uid();
        var versionParsed = int.TryParse(context.User.FindFirst(Jwt.Version)?.Value, out var version);
        if (uid == null || !versionParsed)
        {
            context.Response.StatusCode = 401;
            return;
        }

        var versionOk = await db.Users.Have(x => x.Id == uid && x.Version == version);
        if (!versionOk)
        {
            context.Response.StatusCode = 401;
            return;
        }

        await next(context);
    }

    private static bool RequiresAuthentication(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null) return false;

        var authorize = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
        return authorize != null;
    }
}

public static class UserVersionMiddlewareExtensions
{
    public static IApplicationBuilder UseUserVersionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserVersionMiddleware>();
    }
}