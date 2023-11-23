namespace Web.Config;

public class TokenTransferMiddleware
{
    private readonly RequestDelegate next;

    public TokenTransferMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    private const string key = "token";

    public async Task InvokeAsync(HttpContext context)
    {
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

        await next(context);
    }

    //private static bool RequiresAuthentication(HttpContext context)
    //{
    //    var endpoint = context.GetEndpoint();
    //    if (endpoint == null) return false;
    //    var authorize = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
    //    return authorize != null;
    //}
    private static string? Cookie(HttpContext context)
    {
        context.Request.Cookies.TryGetValue(key, out var value);
        return value;
    }
}

public static class TokenTransferMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenTransferMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenTransferMiddleware>();
    }
}