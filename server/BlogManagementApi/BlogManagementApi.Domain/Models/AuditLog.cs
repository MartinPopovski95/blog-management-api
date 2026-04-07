using BlogManagementApi.Domain.Enums;

namespace BlogManagementApi.Domain.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public AuditActionEnum Action { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public string? Changes { get; set; }
    }
}
