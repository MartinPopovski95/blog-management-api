using BlogManagementApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogManagementApi.DataAccess.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            #region Relations
            builder.HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Properties
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(p => p.Slug)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(p => p.Content)
                .IsRequired();
            builder.Property(p => p.Author)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.AuthorImage)
                .HasMaxLength(500);
            builder.Property(p => p.PostImage)
                .HasMaxLength(500);
            builder.Property(p => p.Category)
                .HasConversion<string>();
            builder.Property(p => p.DeletedAt)
                .IsRequired(false);
            #endregion

            builder.HasIndex(p => p.DeletedAt)
                .HasFilter("[DeletedAt] IS NULL");
        }
    }
}
