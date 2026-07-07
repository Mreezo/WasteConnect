using Newtonsoft.Json;

namespace WasteConnect.Models
{
    public class DumpingReport
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string DumpingLocation { get; set; }

        public string City { get; set; } = "Pietermaritzburg";

        public string? ImageUrl { get; set; }

        public string Status { get; set; } = "Pending";

        public string? AssignedCompanyId { get; set; }

        public string? AssignedCompanyUserId { get; set; }

        public string? AssignedCompanyName { get; set; }

        public DateTime? AssignedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int DuplicateCount { get; set; } = 1;

        public string Priority { get; set; } = "Low";

        public DateTime LastReportedAt { get; set; } = DateTime.UtcNow;

        public bool IsMasterReport { get; set; } = true;

        public List<string> AdditionalImageUrls { get; set; } = new();

        public List<string> ReportedUserIds { get; set; } = new();

        public string? MasterReportId { get; set; }

        public string? MasterReportUserId { get; set; }

        public double DumpLatitude { get; set; }
        public double DumpLongitude { get; set; }
       
        public string? AfterCleanupImageUrl { get; set; }

        public string? CompletionNotes { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? CleanupStartDate { get; set; }
        public DateTime? CleanupDueDate { get; set; }
        public string? CleanupInstructions { get; set; }



    }
}