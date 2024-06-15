using System.Security.Cryptography;
using System.Text;
using WalletAPI.Data;

namespace WalletAPI;

public class WalletAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _xDiggestKey;

    public WalletAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _xDiggestKey = configuration["XDiggestKey"]!;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-UserId", out var userId) ||
            !context.Request.Headers.TryGetValue("X-Digest", out var digest))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing authentication headers");
            
            return;
        }

        var db = context.RequestServices.GetRequiredService<WalletDbContext>();

        if (!db.Wallets.Any(it => it.UserId == userId.ToString()))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid user id");
            
            return;
        }
        
        context.Request.EnableBuffering();
        var stringBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_xDiggestKey)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringBody));
            var computedDigest = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (digest != computedDigest)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid digest");
                return;
            }
        }

        await _next(context);
    }
}