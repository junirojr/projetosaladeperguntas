using Rocketq.Shared.Primitives;

namespace Rocketq.Rooms.Room;

public class RoomEntity : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Hash BCrypt da senha da sala.
    /// A senha NUNCA é retornada em respostas de API (INV-002).
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    private RoomEntity() { }

    public static RoomEntity Create(string name, Guid ownerId, string passwordHash)
    {
        return new RoomEntity
        {
            Name = name,
            OwnerId = ownerId,
            PasswordHash = passwordHash
        };
    }
}
