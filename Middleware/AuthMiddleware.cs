using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _jwtSecret;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
        DotNetEnv.Env.Load();
        _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "";
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (path.Contains("/api/auth/login") || path.Contains("/api/auth/register")))
            {
                await _next(context);
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };
        
            var token = context.Request.Headers["Authorization"]!;
            if(string.IsNullOrEmpty(token)) throw new ArgumentNullException("Token is missing.");

            if(token.ToString().StartsWith("Bearer")) token = token.ToString().Split(" ").Last().TrimStart();

            var verified = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
            if(verified == null) throw new ArgumentException("Token is malformed.");
            context.User = verified;

            await _next(context);
        }
        catch(ArgumentException e)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Token Error: " + e.Message);
            return;
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Token Error: " + e.Message);
            return;
        }
    }
}