using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Rocketq.Identity.Infrastructure;
using Rocketq.Identity.Persistence;
using Rocketq.Identity.User;

namespace Rocketq.Identity.Endpoints.Login;

public class Endpoint(IdentityDbContext db, JwtService jwtService)
    : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        UserEntity? user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        if (user is null || !user.VerifyPassword(req.Password))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        string token = jwtService.GenerateToken(user.Id, user.Email, user.Username);
        await SendAsync(new LoginResponse(user.Id, user.Username, user.Email, token), cancellation: ct);
    }
}