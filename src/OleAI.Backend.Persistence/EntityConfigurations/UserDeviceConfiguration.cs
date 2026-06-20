using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.EntityConfigurations
{
    public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            builder.ToTable("UserDevices");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.DeviceToken)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(d => d.Platform)
                .IsRequired();

            builder.HasOne(d => d.User)
                .WithMany(u => u.UserDevices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => new { d.UserId, d.DeviceToken }).IsUnique();
        }
    }
}
