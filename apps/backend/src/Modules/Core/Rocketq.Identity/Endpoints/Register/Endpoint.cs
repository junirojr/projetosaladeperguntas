using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Rocketq.Identity.Infrastructure;
using Rocketq.Identity.Persistence;
using Rocketq.Identity.User;

namespace Rocketq.Identity.Endpoints.Register;

public class Endpoint(IdentityDbContext db, JwtService jwtService)
    : Endpoint<RegisterRequest, RegisterResponse>
{
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        bool emailTaken = await db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (emailTaken)
        {
            AddError(r => r.Email, "Email já cadastrado.");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
        UserEntity user = UserEntity.Create(req.Username, req.Email, passwordHash);

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        string token = jwtService.GenerateToken(user.Id, user.Email, user.Username);
        await SendAsync(new RegisterResponse(user.Id, user.Username, user.Email, token), 201, ct);
    }
}