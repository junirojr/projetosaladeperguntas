using Microsoft.EntityFrameworkCore;
using Rocketq.Rooms.Room;
using Rocketq.Rooms.RoomMembership;

namespace Rocketq.Rooms.Persistence;

public class RoomsDbContext(DbContextOptions<RoomsDbContext> options) : DbContext(options)
{
    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<RoomMembershipEntity> RoomMemberships => Set<RoomMembershipEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("rooms");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoomsDbContext).Assembly);
    }
}
