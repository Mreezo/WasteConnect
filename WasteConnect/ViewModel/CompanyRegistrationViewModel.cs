using System.ComponentModel.DataAnnotations;

namespace WasteConnect.ViewModels
{
    public class CompanyRegistrationViewModel
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string RegistrationNumber { get; set; }

        [Required]
        public string ContactPerson { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string ServiceArea { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile CompanyRegistration { get; set; }

        [Required]
        public IFormFile WasteLicense { get; set; }

        [Required]
        public IFormFile TaxClearance { get; set; }

        public IFormFile? InsuranceCertificate { get; set; }
    }
}