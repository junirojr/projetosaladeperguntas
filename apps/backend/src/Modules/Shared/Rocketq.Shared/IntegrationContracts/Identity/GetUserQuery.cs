using FastEndpoints;

namespace Rocketq.Shared.IntegrationContracts.Identity;

/// <summary>
/// Retorna dados públicos de um usuário pelo Id.
/// Handler implementado em Rocketq.Identity.
/// </summary>
public class GetUserQuery : ICommand<GetUserResult>
{
    public Guid UserId { get; set; }
}

public class GetUserResult
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
