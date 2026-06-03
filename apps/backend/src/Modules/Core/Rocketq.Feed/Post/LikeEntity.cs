using Rocketq.Shared.Primitives;

namespace Rocketq.Feed.Post;

/// <summary>
/// Curtida de um usuário em um post.
/// A combinação (PostId, UserId) é única — INV-005.
/// </summary>
public class LikeEntity : Entity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private LikeEntity() { }

    public static LikeEntity Create(Guid postId, Guid userId)
    {
        return new LikeEntity
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
