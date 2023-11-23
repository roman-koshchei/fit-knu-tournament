using Data;
using Lib;
using Microsoft.AspNetCore.Authorization;

namespace Web.Config;

public class UnauthorizedMiddleware
{
    private readonly RequestDelegate next;

    public UnauthorizedMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.ToString();
        if (context.Response.StatusCode == 401 && !path.StartsWith("/api"))
        {
            context.Response.Redirect("/");
            return;
        }

        await next(context);
    }
}

public static class UnauthorizedMiddlewareExtensions
{
    public static IApplicationBuilder UseUnauthorizedMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnauthorizedMiddleware>();
    }
}