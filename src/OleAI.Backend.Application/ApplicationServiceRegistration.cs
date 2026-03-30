using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Features.Pantry.Rules;

namespace OleAI.Backend.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddScoped<AuthBusinessRules>();
            services.AddScoped<PantryBusinessRules>();

            return services;
        }
    }
}
