using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModels
{
    public class DumpingReportViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Illegal Dumping Location / Street Address")]
        public string DumpingLocation { get; set; }

        public string City { get; set; } = "Pietermaritzburg";

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        public string? CapturedImageData { get; set; }

        public string DumpLatitude { get; set; }

        public string DumpLongitude { get; set; }

    }
}