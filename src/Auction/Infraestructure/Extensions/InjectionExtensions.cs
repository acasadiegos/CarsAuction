using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Application.Messaging.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Infraestructure.Logging;
using Infraestructure.Messaging.Consumers;
using Infraestructure.Messaging.Publishers;
using Infraestructure.Persistence.Contexts;
using Infraestructure.Persistence.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infraestructure.Extensions;

public static class InjectionExtensions {

    public static WebApplicationBuilder AddInjectionInfraestructure(this WebApplicationBuilder builder)
    {
        var assembly = typeof(AuctionDbContext).Assembly.FullName;

        builder.Configuration.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            ).CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddDbContext<AuctionDbContext>(
            options => options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(assembly)), ServiceLifetime.Scoped
        );

        builder.Services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(10);

                o.UsePostgres();
                o.UseBusOutbox();

            });

            x.AddConsumersFromNamespaceContaining<AuctionFinishedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                    h.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IAppLogger<>), typeof(SerilogAdapter<>));
        builder.Services.AddScoped<IAuctionRepository<BaseEntityResponse<Auction>, AuctionFiltersDto>, AuctionRepository>();
        builder.Services.AddScoped<IMessagerPublisher, MessagerPublisher>();

        return builder;
    }

}
