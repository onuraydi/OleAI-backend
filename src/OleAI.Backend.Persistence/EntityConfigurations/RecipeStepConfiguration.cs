using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations;

public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("RecipeSteps").HasKey(rs => rs.Id);

        builder.Property(rs => rs.StepNumber).IsRequired();
        builder.Property(rs => rs.Instruction).IsRequired().HasMaxLength(1000);
    }
}
