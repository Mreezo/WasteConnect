using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModels
{
    public class CompanyProfileViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string RegistrationNumber { get; set; }

        [Required]
        public string ContactPerson { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string ServiceArea { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}