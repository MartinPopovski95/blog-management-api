using BlogManagementApi.Domain.Enums;

namespace BlogManagementApi.Services.Abstractions
{
    public interface IAuditLogService
    {
        Task LogAsync(
            string entityType,
            string entityId,
            AuditActionEnum action,
            string userId,
            string userEmail,
            string? changes = null);
    }
}
