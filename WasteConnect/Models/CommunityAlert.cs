using Newtonsoft.Json;

namespace WasteConnect.Models
{
    public class CommunityAlert
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string AlertType { get; set; } = string.Empty;

        public string AreaType { get; set; } = string.Empty;

        public int? WardNumber { get; set; }

        public string? SpecificArea { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string Priority { get; set; } = "Normal";

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }
    }
}