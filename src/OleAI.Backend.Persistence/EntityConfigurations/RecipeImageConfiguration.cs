using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations;

public class RecipeImageConfiguration : IEntityTypeConfiguration<RecipeImage>
{
    public void Configure(EntityTypeBuilder<RecipeImage> builder)
    {
        builder.ToTable("RecipeImages").HasKey(ri => ri.Id);

        builder.Property(ri => ri.ImageUrl).IsRequired();
        builder.Property(ri => ri.PublicId).IsRequired();
        builder.Property(ri => ri.IsMain).IsRequired();
        builder.Property(ri => ri.StepId).IsRequired(false);
    }
}
