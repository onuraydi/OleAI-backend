using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Auth.Rules;
using OleAI.Backend.Application.Services.AuthService;

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
            services.AddScoped<RoleBusinessRules>();

            services.AddScoped<IAuthService>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var tokenOptions = configuration.GetSection("TokenOptions").Get<Core.Security.Jwt.TokenOptions>()
                    ?? throw new ArgumentNullException("TokenOptions", "TokenOptions configuration not found.");

                var refreshTokenRepo = sp.GetRequiredService<Services.Repositories.IRefreshTokenRepository>();
                return new AuthManager(refreshTokenRepo, tokenOptions);
            });

            return services;
        }
    }
}
