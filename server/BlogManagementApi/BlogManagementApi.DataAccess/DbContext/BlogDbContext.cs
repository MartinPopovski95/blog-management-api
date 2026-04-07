using BlogManagementApi.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementApi.DataAccess.DbContext
{
    public class BlogDbContext : IdentityDbContext<User, Role, string>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);
            SeedRoles(builder);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(
                new Role { Id = "cb96a83f-3e36-4a2e-b3e6-403d6b9053cc", Name = "Admin",  NormalizedName = "ADMIN",  ConcurrencyStamp = "57f174b2-1434-42e0-91e9-648d37b8be2e" },
                new Role { Id = "29812de0-eada-4e37-9245-8b3419d4e3ca", Name = "Editor", NormalizedName = "EDITOR", ConcurrencyStamp = "5cc70ebe-8476-4cda-a247-efefe1195477" }
            );
        }
    }
}
