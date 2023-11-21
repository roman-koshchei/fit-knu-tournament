using Backend.Auth;
using Data;
using Lib;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Config;

public class UserVersionMiddleware
{
    private readonly RequestDelegate next;
    private readonly Db db;

    public UserVersionMiddleware(RequestDelegate next, Db db)
    {
        this.next = next;
        this.db = db;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var uid = context.User.Uid();
        var version = context.User.FindFirstValue(Jwt.Version);
        if (uid == null || version == null)
        {
            context.Response.StatusCode = 401;
            return;
        }

        //var versionOk = await db.Users.Have(x => x.Id == version && x.Version == version);

        //if (!domainAllowed)
        //{
        //    context.Response.StatusCode = 403;
        //    return;
        //}

        await next(context);
    }
}