using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rocketq.Identity.Persistence;
using Testcontainers.PostgreSql;

namespace Rocketq.Tests;

/// <summary>
/// WebApplicationFactory base para testes de integração.
/// Inicia um PostgreSQL via Testcontainers e substitui a connection string.
/// </summary>
public class AppFactory : AppFixture<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("rocketq_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override async ValueTask PreSetupAsync()
    {
        await _postgres.StartAsync();
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", _postgres.GetConnectionString());
        builder.UseSetting("Jwt:Secret", "test-secret-minimum-32-characters-ok!");
        builder.UseSetting("Jwt:Issuer", "rocketq");
        builder.UseSetting("Jwt:Audience", "rocketq-ui");
        builder.UseSetting("Cors:AllowedOrigins:0", "http://localhost:3000");
    }

    protected override async ValueTask SetupAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        IdentityDbContext db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await db.Database.MigrateAsync();
    }

    protected override async ValueTask TearDownAsync()
    {
        await _postgres.DisposeAsync();
    }
}
