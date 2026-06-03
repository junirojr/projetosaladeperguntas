using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocketq.Rooms.Persistence;

namespace Rocketq.Rooms;

public static class DependencyInjection
{
    public static IServiceCollection AddRoomsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<RoomsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        return services;
    }
}
