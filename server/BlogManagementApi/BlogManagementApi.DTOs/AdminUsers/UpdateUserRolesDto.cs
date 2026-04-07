using System.ComponentModel.DataAnnotations;

namespace BlogManagementApi.DTOs.AdminUsers
{
    public class UpdateUserRolesDto
    {
        [Required]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
