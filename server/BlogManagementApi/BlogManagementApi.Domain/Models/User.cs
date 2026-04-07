using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace BlogManagementApi.Domain.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAtUtc { get; set; }
        [JsonIgnore]
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
