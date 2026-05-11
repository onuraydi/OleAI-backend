namespace OleAI.Backend.Persistence.Contexts;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Domain.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<IngredientAlias> IngredientAliases { get; set; }
    public DbSet<UserIngredient> UserIngredients { get; set; }
    public DbSet<IngredientRequest> IngredientRequests { get; set; }
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeImage> RecipeImages { get; set; }
    public DbSet<RecipeStep> RecipeSteps { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<RecipeInteraction> RecipeInteractions { get; set; }
    public DbSet<RecipeComment> RecipeComments { get; set; }
    public DbSet<DietaryPreference> DietaryPreferences { get; set; }
    public DbSet<AllergyRestriction> AllergyRestrictions { get; set; }
    public DbSet<UserDietaryPreference> UserDietaryPreferences { get; set; }
    public DbSet<UserAllergy> UserAllergies { get; set; }
    public DbSet<UserDislikedIngredient> UserDislikedIngredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // çalışan Assembly (proje) içindeki tüm IEntityTypeConfiguration arayüzünü uygulayan
        // sınıfları bulmasını ve otomatik olarak Apply etmesini söylüyoruz.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

    }
}
