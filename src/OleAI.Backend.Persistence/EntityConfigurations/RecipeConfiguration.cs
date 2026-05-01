using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToTable("Recipes").HasKey(r => r.Id);

            builder.Property(r => r.Title).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Description).HasMaxLength(2000);
            builder.Property(r => r.Visibility).IsRequired();
            builder.Property(r => r.Status).IsRequired();

            builder.HasMany(r => r.RecipeIngredients)
                .WithOne()
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(r => r.RecipeIngredients).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(r => r.RecipeSteps)
                .WithOne()
                .HasForeignKey(rs => rs.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(r => r.RecipeSteps).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(r => r.RecipeImages)
                .WithOne()
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(r => r.RecipeImages).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
