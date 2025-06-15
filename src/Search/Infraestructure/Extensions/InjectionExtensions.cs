using Infraestructure.Interfaces;
using Infraestructure.Interfaces.Client;
using Infraestructure.Persistence.Interfaces;
using Infraestructure.Persistence.Repositories;
using Infraestructure.Services;
using Infraestructure.Services.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly;

namespace Infraestructure.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionInfraestructure(this IServiceCollection services)
        {

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
