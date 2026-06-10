using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Persistence.Contexts;
using OleAI.Backend.Persistence.Repositories;

namespace OleAI.Backend.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("OleAIConnectionString")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
            services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IIngredientAliasRepository, IngredientAliasRepository>();
            services.AddScoped<IUserIngredientRepository, UserIngredientRepository>();
            services.AddScoped<IIngredientRequestRepository, IngredientRequestRepository>();
            services.AddScoped<IShoppingListItemRepository, ShoppingListItemRepository>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IRecipeInteractionRepository, RecipeInteractionRepository>();
            services.AddScoped<IRecipeCommentRepository, RecipeCommentRepository>();
            services.AddScoped<IDietaryPreferenceRepository, DietaryPreferenceRepository>();
            services.AddScoped<IAllergyRestrictionRepository, AllergyRestrictionRepository>();
            services.AddScoped<IUserDietaryPreferenceRepository, UserDietaryPreferenceRepository>();
            services.AddScoped<IUserAllergyRepository, UserAllergyRepository>();
            services.AddScoped<IUserDislikedIngredientRepository, UserDislikedIngredientRepository>();
            services.AddScoped<IRecipeCategoryRepository, RecipeCategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();

            return services;
        }
    }
}
