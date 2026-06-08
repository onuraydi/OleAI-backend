using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories").HasKey(c => c.Id);

            builder.Property(c => c.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            builder.Property(c => c.IconName).HasColumnName("IconName").IsRequired().HasMaxLength(50);

            builder.HasMany(c => c.Ingredients)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Kategori silinse bile o kategoriye ait ürünler silinmez.
        }
    }
}
