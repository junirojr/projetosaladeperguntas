using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Rocketq.Identity.Persistence;
using Rocketq.Identity.User;

namespace Rocketq.Identity.Endpoints.GetProfile;

public class Endpoint(IdentityDbContext db) : EndpointWithoutRequest<GetProfileResponse>
{
    public override void Configure()
    {
        Get("/users/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Guid userId = Guid.Parse(User.FindFirst("userId")!.Value);

        UserEntity? user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new GetProfileResponse(user.Id, user.Username, user.Email), cancellation: ct);
    }
}