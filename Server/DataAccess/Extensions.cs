using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration, string connectionString)
    {
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddDbContext<AppContext>(x =>
            x.UseNpgsql(connectionString));
        return services;
    }
}