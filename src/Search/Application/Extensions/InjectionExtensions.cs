using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services)
        {
            services.AddScoped<IItemApplication, ItemApplication>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        } 
    }
}
