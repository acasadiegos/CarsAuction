using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Domain.Repositories;
using Infraestructure.Interfaces;
using Infraestructure.Interfaces.Client;
using Infraestructure.Messaging.Consumers;
using Infraestructure.Persistence.Repositories;
using Infraestructure.Services;
using Infraestructure.Services.Client.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Reflection;

namespace Infraestructure.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionInfraestructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
                x.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
                x.AddConsumersFromNamespaceContaining<AuctionDeletedConsumer>();

                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMq:Host"], "/", h =>
                    {
                        h.Username(configuration.GetValue("RabbitMq:Username","guest"));
                        h.Password(configuration.GetValue("RabbitMq:Password","guest"));
                    });

                    cfg.ReceiveEndpoint("search-auction-created", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(5, 5));

                        e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("search-auction-updated", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(5, 5));

                        e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("search-auction-deleted", e =>
                    {
                        e.UseMessageRetry(r => r.Interval(5, 5));

                        e.ConfigureConsumer<AuctionDeletedConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IItemRepository<BaseEntityResponse<Item>, ItemFiltersDto>, ItemRepository>();
            services.AddScoped<IItemService, ItemService>();
            services.AddHttpClient<IHttpClientService, HttpClientService>().AddPolicyHandler(GetPolicy());

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetPolicy()
                => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
    }
}
