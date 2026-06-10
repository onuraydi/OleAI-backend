using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations;

public class RecipeCategoryMappingConfiguration : IEntityTypeConfiguration<RecipeCategoryMapping>
{
    public void Configure(EntityTypeBuilder<RecipeCategoryMapping> builder)
    {
        builder.ToTable("RecipeCategoryMappings").HasKey(rcm => rcm.Id);

        builder.HasIndex(rcm => new { rcm.RecipeId, rcm.RecipeCategoryId }).IsUnique();

        builder.HasOne(rcm => rcm.RecipeCategory)
            .WithMany(rc => rc.RecipeCategories)
            .HasForeignKey(rcm => rcm.RecipeCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
