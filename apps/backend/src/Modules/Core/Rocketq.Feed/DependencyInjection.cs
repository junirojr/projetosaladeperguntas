using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocketq.Feed.Persistence;

namespace Rocketq.Feed;

public static class DependencyInjection
{
    public static IServiceCollection AddFeedModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FeedDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        return services;
    }
}
