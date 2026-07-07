using Newtonsoft.Json;

namespace WasteConnect.Models
{
    public class Company
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; }

        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }

        public string ContactPerson { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string ServiceArea { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string CompanyRegistrationUrl { get; set; }

        public string WasteLicenseUrl { get; set; }

        public string TaxClearanceUrl { get; set; }

        public string? InsuranceCertificateUrl { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime? ReviewedAt { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}