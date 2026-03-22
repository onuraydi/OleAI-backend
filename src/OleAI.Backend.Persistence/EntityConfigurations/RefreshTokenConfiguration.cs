using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens").HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token).IsRequired().HasMaxLength(200);
            builder.HasIndex(rt => rt.Token).IsUnique();

            builder.Property(rt => rt.CreatedByIp).IsRequired().HasMaxLength(50);
            builder.Property(rt => rt.RevokedByIp).HasMaxLength(50);
            builder.Property(rt => rt.ReplacedByToken).HasMaxLength(200);
            builder.Property(rt => rt.ReasonRevoked).HasMaxLength(200);
        }
    }
}
