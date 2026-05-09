using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations;

public class UserDislikedIngredientConfiguration : IEntityTypeConfiguration<UserDislikedIngredient>
{
    public void Configure(EntityTypeBuilder<UserDislikedIngredient> builder)
    {
        builder.ToTable("UserDislikedIngredients");
        builder.HasKey(ui => ui.Id);

        // Bir kullanıcı aynı malzemeyi birden fazla kez "sevilmeyen" olarak ekleyemez
        builder.HasIndex(ui => new { ui.UserId, ui.IngredientId }).IsUnique();

        builder.HasOne(ui => ui.User)
            .WithMany(u => u.UserDislikedIngredients)
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ui => ui.Ingredient)
            .WithMany(i => i.UserDislikedIngredients)
            .HasForeignKey(ui => ui.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
