using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class IngredientRequestConfiguration : IEntityTypeConfiguration<IngredientRequest>
    {
        public void Configure(EntityTypeBuilder<IngredientRequest> builder)
        {
            builder.ToTable("IngredientRequests").HasKey(r => r.Id);

            builder.Property(r => r.RequestedName).HasColumnName("RequestedName").IsRequired().HasMaxLength(150);
            builder.Property(r => r.Status).HasColumnName("Status").IsRequired();
            builder.Property(r => r.LlmAnalysisResult).HasColumnName("LlmAnalysisResult").HasColumnType("nvarchar(max)"); // JSON tutacağı için

            // Adminlerin bekleyen talepleri hızlı görmesi için statüye index
            builder.HasIndex(r => r.Status).HasDatabaseName("IX_IngredientRequests_Status");
        }
    }
}
