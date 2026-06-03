using Rocketq.Shared.Primitives;

namespace Rocketq.Rooms.RoomMembership;

/// <summary>
/// Representa o vínculo ativo entre um usuário e uma sala.
/// A existência deste registro é o gate de acesso ao Feed (INV-001).
/// </summary>
public class RoomMembershipEntity : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private RoomMembershipEntity() { }

    public static RoomMembershipEntity Create(Guid userId, Guid roomId)
    {
        return new RoomMembershipEntity
        {
            UserId = userId,
            RoomId = roomId,
            JoinedAt = DateTime.UtcNow
        };
    }
}
