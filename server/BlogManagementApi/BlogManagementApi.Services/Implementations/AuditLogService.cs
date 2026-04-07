using BlogManagementApi.DataAccess.DbContext;
using BlogManagementApi.Domain.Enums;
using BlogManagementApi.Domain.Models;
using BlogManagementApi.Services.Abstractions;

namespace BlogManagementApi.Services.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly BlogDbContext _context;

        public AuditLogService(BlogDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            string entityType,
            string entityId,
            AuditActionEnum action,
            string userId,
            string userEmail,
            string? changes = null)
        {
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                UserId = userId,
                UserEmail = userEmail,
                OccurredAt = DateTime.UtcNow,
                Changes = changes
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
