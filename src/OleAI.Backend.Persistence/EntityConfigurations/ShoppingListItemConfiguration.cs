using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
        {
            builder.ToTable("ShoppingListItems").HasKey(s => s.Id);

            builder.Property(s => s.Id).HasColumnName("Id").IsRequired();
            builder.Property(s => s.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(s => s.IngredientId).HasColumnName("IngredientId").IsRequired(false);
            builder.Property(s => s.IngredientRequestId).HasColumnName("IngredientRequestId").IsRequired(false);
            builder.Property(s => s.Quantity).HasColumnName("Quantity").IsRequired();
            builder.Property(s => s.Unit).HasColumnName("Unit").IsRequired();
            builder.Property(s => s.IsPurchased).HasColumnName("IsPurchased").IsRequired();


            builder.HasOne(s => s.User)
                   .WithMany()
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Ingredient)
                   .WithMany(i => i.ShoppingListItems)
                   .HasForeignKey(s => s.IngredientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.IngredientRequest)
                   .WithMany()
                   .HasForeignKey(s => s.IngredientRequestId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
