using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Services.Llm;

namespace OleAI.Backend.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddScoped<AuthBusinessRules>();
            services.AddScoped<PantryBusinessRules>();
            services.AddScoped<CategoryBusinessRules>();

            services.Configure<OllamaSettings>(configuration.GetSection("OllamaSettings"));
            services.AddHttpClient<ILocalLlmService, OllamaLocalLlmManager>();

            return services;
        }
    }
}
