using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Rocketq.Rooms.Persistence;
using Rocketq.Shared.IntegrationContracts.Rooms;

namespace Rocketq.Rooms.CommandHandlers;

/// <summary>
/// Handler do IntegrationContract CheckMembershipQuery.
/// Ownership: Rocketq.Rooms.
/// Consumido por: Rocketq.Feed (INV-001 — membership gate).
/// </summary>
public class CheckMembershipHandler(RoomsDbContext db)
    : ICommandHandler<CheckMembershipQuery, CheckMembershipResult>
{
    public async Task<CheckMembershipResult> ExecuteAsync(
        CheckMembershipQuery query, CancellationToken ct)
    {
        var isMember = await db.RoomMemberships
            .AnyAsync(m => m.UserId == query.UserId && m.RoomId == query.RoomId, ct);

        return new CheckMembershipResult { IsMember = isMember };
    }
}
