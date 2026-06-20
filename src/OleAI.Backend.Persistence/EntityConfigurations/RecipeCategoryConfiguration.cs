using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations;

public class RecipeCategoryConfiguration : IEntityTypeConfiguration<RecipeCategory>
{
    public void Configure(EntityTypeBuilder<RecipeCategory> builder)
    {
        builder.ToTable("RecipeCategories").HasKey(rc => rc.Id);
        builder.Property(rc => rc.Name).IsRequired().HasMaxLength(50);
        builder.Property(rc => rc.IconName).HasMaxLength(50);
    }
}
