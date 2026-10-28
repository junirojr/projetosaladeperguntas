using FastEndpoints.Security;
using Microsoft.Extensions.Configuration;

namespace Rocketq.Identity.Infrastructure;

public class JwtService(IConfiguration configuration)
{
    public string GenerateToken(Guid userId, string email, string username)
    {
        return JwtBearer.CreateToken(opts =>
        {
            opts.SigningKey = configuration["Jwt:Secret"]!;
            opts.ExpireAt = DateTime.UtcNow.AddDays(7);
            opts.User.Claims.Add(("userId", userId.ToString()));
            opts.User.Claims.Add(("email", email));
            opts.User.Claims.Add(("username", username));
        });
    }
}