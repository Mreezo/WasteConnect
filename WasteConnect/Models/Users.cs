using Microsoft.AspNetCore.Identity;

namespace WasteConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? HomeAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}