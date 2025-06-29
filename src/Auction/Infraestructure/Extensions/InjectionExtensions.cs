using Infraestructure.Messaging.Interfaces;
using Infraestructure.Messaging.Publishers;
using Infraestructure.Persistence.Contexts;
using Infraestructure.Persistence.Interfaces;
using Infraestructure.Persistence.Repositories;
using MassTransit;
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

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(10);

                o.UsePostgres();
                o.UseBusOutbox();

            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IMessagerPublisher, MessagerPublisher>();

        return services;
    }

}
