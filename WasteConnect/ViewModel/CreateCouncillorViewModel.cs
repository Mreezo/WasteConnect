using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModel
{
    public class CreateCouncillorViewModel
    {


        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ward Number")]
        [Range(1, 100)]
        public int WardNumber { get; set; }

        [Display(Name = "Position")]
        public string PositionTitle { get; set; } = "Ward Councillor";
    }
}
