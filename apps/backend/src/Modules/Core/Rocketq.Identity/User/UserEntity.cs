using Rocketq.Shared.Primitives;

namespace Rocketq.Identity.User;

public class UserEntity : AuditableEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Senha armazenada com hash BCrypt. Nunca expor em respostas de API (INV-003).
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    private UserEntity() { }

    public static UserEntity Create(string username, string email, string passwordHash)
    {
        return new UserEntity
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash
        };
    }
}
