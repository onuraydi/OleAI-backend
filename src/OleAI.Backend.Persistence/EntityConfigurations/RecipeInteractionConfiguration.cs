using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations;

public class RecipeInteractionConfiguration : IEntityTypeConfiguration<RecipeInteraction>
{
    public void Configure(EntityTypeBuilder<RecipeInteraction> builder)
    {
        builder.ToTable("RecipeInteractions").HasKey(i => i.Id);

        builder.Property(i => i.Id).HasColumnName("Id").IsRequired();
        builder.Property(i => i.RecipeId).HasColumnName("RecipeId").IsRequired();
        builder.Property(i => i.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(i => i.IsLiked).HasColumnName("IsLiked").IsRequired();
        builder.Property(i => i.IsDisliked).HasColumnName("IsDisliked").IsRequired();
        builder.Property(i => i.IsSaved).HasColumnName("IsSaved").IsRequired();

        builder.HasIndex(i => new { i.RecipeId, i.UserId }).IsUnique();

        builder.HasOne(i => i.Recipe)
            .WithMany(r => r.Interactions)
            .HasForeignKey(i => i.RecipeId)
            .OnDelete(DeleteBehavior.Cascade); // Tarif silinirse etkileşimler de silinsin

        builder.HasOne(i => i.User)
            .WithMany(u => u.Interactions)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
