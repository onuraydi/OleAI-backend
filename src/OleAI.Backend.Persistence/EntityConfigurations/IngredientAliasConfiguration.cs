using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class IngredientAliasConfiguration : IEntityTypeConfiguration<IngredientAlias>
    {
        public void Configure(EntityTypeBuilder<IngredientAlias> builder)
        {
            builder.ToTable("IngredientAliases").HasKey(a => a.Id);

            builder.Property(a => a.AliasName).HasColumnName("AliasName").IsRequired().HasMaxLength(100);

            // Performans için index atıyoruz
            builder.HasIndex(a => a.AliasName).HasDatabaseName("IX_IngredientAliases_AliasName");
        }
    }
}
