using Infraestructure.Interfaces;
using Infraestructure.Interfaces.Client;
using Infraestructure.Persistence.Interfaces;
using Infraestructure.Persistence.Repositories;
using Infraestructure.Services;
using Infraestructure.Services.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly;
using MassTransit;
using System.Reflection;
using Infraestructure.Messaging.Consumers;

namespace Infraestructure.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionInfraestructure(this IServiceCollection services)
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
            services.AddScoped<IItemRepository, ItemRepository>();
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
