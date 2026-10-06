namespace WasteConnect.Models
{
    public class CommunityAlertSmsMessage
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string AlertId { get; set; } = string.Empty;
    }
}