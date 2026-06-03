using Microsoft.EntityFrameworkCore;
using Rocketq.Feed.Post;
using Rocketq.Feed.Comment;

namespace Rocketq.Feed.Persistence;

public class FeedDbContext(DbContextOptions<FeedDbContext> options) : DbContext(options)
{
    public DbSet<PostEntity> Posts => Set<PostEntity>();
    public DbSet<LikeEntity> Likes => Set<LikeEntity>();
    public DbSet<CommentEntity> Comments => Set<CommentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("feed");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FeedDbContext).Assembly);
    }
}
