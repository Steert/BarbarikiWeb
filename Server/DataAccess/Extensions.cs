using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddDbContext<AppContext>(x =>
            x.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}