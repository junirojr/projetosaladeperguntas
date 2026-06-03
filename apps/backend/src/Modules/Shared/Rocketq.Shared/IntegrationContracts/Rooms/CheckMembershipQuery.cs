using FastEndpoints;

namespace Rocketq.Shared.IntegrationContracts.Rooms;

/// <summary>
/// Verifica se um usuário é membro ativo de uma sala.
/// Handler implementado em Rocketq.Rooms.
/// Consumido por Rocketq.Feed em todo endpoint (INV-001).
/// </summary>
public class CheckMembershipQuery : ICommand<CheckMembershipResult>
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
}

public class CheckMembershipResult
{
    public bool IsMember { get; set; }
}
