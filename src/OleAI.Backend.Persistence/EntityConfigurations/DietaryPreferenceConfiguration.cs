using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class DietaryPreferenceConfiguration : IEntityTypeConfiguration<DietaryPreference>
    {
        public void Configure(EntityTypeBuilder<DietaryPreference> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(d => d.Name).IsUnique();

            // Dışlanan malzemeler
            builder.HasMany(d => d.ExcludedIngredients)
                .WithOne(dei => dei.DietaryPreference)
                .HasForeignKey(dei => dei.DietaryPreferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // dışlanan kategoriler listesi
            builder.HasMany(d => d.ExcludedCategories)
                .WithOne(dei => dei.DietaryPreference)
                .HasForeignKey(dei => dei.DietaryPreferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // kullanıcı
            builder.HasMany(d => d.UserDietaryPreferences)
                .WithOne(udp => udp.DietaryPreference)
                .HasForeignKey(udp => udp.DietaryPreferenceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
