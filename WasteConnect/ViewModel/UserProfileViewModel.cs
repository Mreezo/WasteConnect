using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModels
{
    public class UserProfileViewModel
    {
        public string? ProfileImageUrl { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Home Address")]
        public string? HomeAddress { get; set; }

        public string Email { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}