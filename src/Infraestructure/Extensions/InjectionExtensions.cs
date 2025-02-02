using Infraestructure.Persistence.Contexts;
using Infraestructure.Persistence.Interfaces;
using Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Extensions;

public static class InjectionExtensions {

    public static IServiceCollection AddInjectionInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(AuctionDbContext).Assembly.FullName;

        services.AddDbContext<AuctionDbContext>(
            options => options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(assembly)), ServiceLifetime.Scoped
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();

        return services;
    }

}
