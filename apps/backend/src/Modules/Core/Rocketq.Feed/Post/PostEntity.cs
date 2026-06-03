using Rocketq.Shared.Primitives;

namespace Rocketq.Feed.Post;

public class PostEntity : AuditableEntity
{
    public Guid RoomId { get; private set; }

    /// <summary>
    /// Apenas o AuthorId pode editar ou deletar este post (INV-004).
    /// </summary>
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; } = string.Empty;

    public ICollection<LikeEntity> Likes { get; private set; } = [];

    private PostEntity() { }

    public static PostEntity Create(Guid roomId, Guid authorId, string content)
    {
        return new PostEntity
        {
            RoomId = roomId,
            AuthorId = authorId,
            Content = content
        };
    }
}
