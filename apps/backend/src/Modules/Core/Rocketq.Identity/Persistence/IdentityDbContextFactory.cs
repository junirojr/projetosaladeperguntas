using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Rocketq.Identity.Persistence;

// Usado apenas pelo dotnet-ef em design-time (migrations). Nunca em produção.
public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<IdentityDbContext> opts = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql("Host=localhost;Database=rocketq;Username=postgres;Password=postgres")
            .Options;
        return new IdentityDbContext(opts);
    }
}