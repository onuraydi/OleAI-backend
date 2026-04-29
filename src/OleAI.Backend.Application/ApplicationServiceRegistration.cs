using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Auth.Constants;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Features.Ingredients.Rules;
using OleAI.Backend.Application.Features.Pantry.Rules;
using OleAI.Backend.Application.Features.Recipes.Rules;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.AuthService;
using OleAI.Backend.Application.Services.ImageStorage;
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


            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));

            services.AddScoped<AuthBusinessRules>();
            services.AddScoped<RoleBusinessRules>();

            services.AddScoped<IAuthService>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var tokenOptions = configuration.GetSection("TokenOptions").Get<Core.Security.Jwt.TokenOptions>()
                    ?? throw new ArgumentNullException("TokenOptions", "TokenOptions configuration not found.");

                var refreshTokenRepo = sp.GetRequiredService<Services.Repositories.IRefreshTokenRepository>();
                return new AuthManager(refreshTokenRepo, tokenOptions);
            });
            services.AddScoped<PantryBusinessRules>();
            services.AddScoped<CategoryBusinessRules>();
            services.AddScoped<IngredientBusinessRules>();
            services.AddScoped<IngredientAliasBusinessRules>();
            services.AddScoped<ShoppingListBusinessRules>();
            services.AddScoped<RecipeBusinessRules>();

            services.Configure<OllamaSettings>(configuration.GetSection("OllamaSettings"));
            services.AddHttpClient<ILocalLlmService, OllamaLocalLlmManager>();
            services.AddScoped<IImageService, CloudinaryImageManager>();

            return services;
        }
    }
}
