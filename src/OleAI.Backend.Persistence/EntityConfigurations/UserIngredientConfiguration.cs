using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class UserIngredientConfiguration : IEntityTypeConfiguration<UserIngredient>
    {
        public void Configure(EntityTypeBuilder<UserIngredient> builder)
        {
            builder.ToTable("UserIngredients").HasKey(ui => ui.Id);

            builder.Property(ui => ui.Amount).HasColumnName("Amount").IsRequired();
            builder.Property(ui => ui.Unit).HasColumnName("Unit").IsRequired().HasConversion<int>();

            // Performans Kritik İndeks: Kullanıcının dolabını çekerken sürekli UserId'ye göre sorgu atacağız.
            builder.HasIndex(u => u.UserId).HasDatabaseName("IX_UserIngredients_UserId");

            // Aynı malzemeden dolapta iki tane olmamasını garanti altına almak için Composite Unique Index atabiliriz
            // Ancak onay bekleyen (IngredientId = null) durumlar olacağı için sadece filtreli index kullanıyoruz:
            builder.HasIndex(u => new { u.UserId, u.IngredientId, u.IngredientAliasId })
                .IsUnique()
                .HasFilter("[IngredientId] IS NOT NULL")
                .HasDatabaseName("UK_UserIngredients_UserId_IngredientId_AliasId");

            builder.HasOne(ui => ui.User)
                .WithMany(u => u.UserIngredients)
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ui => ui.Ingredient)
                .WithMany(i => i.UserIngredients)
                .HasForeignKey(ui => ui.IngredientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(ui => ui.IngredientAlias)
                .WithMany()
                .HasForeignKey(ui => ui.IngredientAliasId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
