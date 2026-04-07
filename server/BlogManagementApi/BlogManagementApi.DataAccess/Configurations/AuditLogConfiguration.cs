using BlogManagementApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogManagementApi.DataAccess.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(a => a.Action)
                .HasConversion<string>();

            builder.Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(a => a.UserEmail)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(a => a.Changes)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(a => a.OccurredAt);
            builder.HasIndex(a => new { a.EntityType, a.EntityId });
        }
    }
}
