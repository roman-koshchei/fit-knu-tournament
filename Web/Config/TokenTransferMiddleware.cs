namespace Web.Config;

/// <summary>
/// Middleware for transferring a token from a cookie to the Authorization header.
/// </summary>
public class TokenTransferMiddleware
{
    private readonly RequestDelegate next;

    public TokenTransferMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    // Key for accessing the token from the cookie.
    private const string key = "token";

    /// <summary>
    /// Transfers the token from the cookie to the Authorization header.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Retrieve the token from the cookie.
        var cookie = Cookie(context);
        Console.WriteLine(cookie);
        if (cookie == null)
        {
            await next(context);
            return;
        }
        if (context.Request.Headers.Authorization.Count == 0)
        {
            context.Request.Headers.Remove("Authorization");
            context.Request.Headers.Add("Authorization", $"Bearer {cookie}");

            Console.WriteLine($"Bearer {cookie}");
        }

        // Continue processing the request.
        await next(context);
    }

    // Retrieves the token from the cookie.
    private static string? Cookie(HttpContext context)
    {
        context.Request.Cookies.TryGetValue(key, out var value);
        return value;
    }
}

/// <summary>
/// Extension methods for integrating the TokenTransferMiddleware into the application's request processing pipeline.
/// </summary>
public static class TokenTransferMiddlewareExtensions
{
    /// <summary>
    /// Adds the TokenTransferMiddleware to the middleware pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder with the TokenTransferMiddleware added.</returns>
    public static IApplicationBuilder UseTokenTransferMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenTransferMiddleware>();
    }
}
