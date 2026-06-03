using Rocketq.Shared.Primitives;

namespace Rocketq.Feed.Comment;

public class CommentEntity : AuditableEntity
{
    public Guid PostId { get; private set; }

    /// <summary>
    /// Apenas o AuthorId pode deletar este comentário (INV-004).
    /// </summary>
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; } = string.Empty;

    private CommentEntity() { }

    public static CommentEntity Create(Guid postId, Guid authorId, string content)
    {
        return new CommentEntity
        {
            PostId = postId,
            AuthorId = authorId,
            Content = content
        };
    }
}
