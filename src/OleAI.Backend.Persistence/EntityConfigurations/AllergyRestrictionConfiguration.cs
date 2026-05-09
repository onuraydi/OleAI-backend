using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.Configurations;

public class AllergyRestrictionConfiguration : IEntityTypeConfiguration<AllergyRestriction>
{
    public void Configure(EntityTypeBuilder<AllergyRestriction> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(a => a.Name).IsUnique();

        // Dışlanan Malzemeler İlişkisi
        builder.HasMany(a => a.ExcludedIngredients)
               .WithOne(aei => aei.AllergyRestriction)
               .HasForeignKey(aei => aei.AllergyRestrictionId)
               .OnDelete(DeleteBehavior.Cascade);

        // Dışlanan Kategoriler İlişkisi
        builder.HasMany(a => a.ExcludedCategories)
               .WithOne(aec => aec.AllergyRestriction)
               .HasForeignKey(aec => aec.AllergyRestrictionId)
               .OnDelete(DeleteBehavior.Cascade);

        // Kullanıcı İlişkisi
        builder.HasMany(a => a.UserAllergies)
               .WithOne(ua => ua.AllergyRestriction)
               .HasForeignKey(ua => ua.AllergyRestrictionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
