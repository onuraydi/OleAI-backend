using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.ToTable("Ingredients").HasKey(i => i.Id);

            builder.Property(i => i.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            builder.Property(i => i.BaseUnitType).HasColumnName("BaseUnitType").IsRequired().HasMaxLength(50);

            builder.HasMany(i => i.Aliases)
                .WithOne(a => a.Ingredient)
                .HasForeignKey(a => a.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
