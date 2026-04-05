using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
    {
        public void Configure(EntityTypeBuilder<OperationClaim> builder)
        {
            builder.ToTable("OperationClaims").HasKey(oc => oc.Id);

            builder.Property(oc => oc.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(oc => oc.Name).IsUnique();

            OperationClaim[] operationClaimsSeed = {
                new() { Id = Guid.Parse("5f9b3b1e-7b1e-4b1e-8b1e-7b1e4b1e5f9b"), Name = "User" },
                new() { Id = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"), Name = "Admin" }
            };

            builder.HasData(operationClaimsSeed);
        }
    }
}
