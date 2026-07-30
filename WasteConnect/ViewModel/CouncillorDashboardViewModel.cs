using WasteConnect.Models;

namespace WasteConnect.ViewModels
{
    public class CouncillorDashboardViewModel
    {
        public string CouncillorName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PositionTitle { get; set; } = "Ward Councillor";

        public int WardNumber { get; set; }

        public List<DumpingReport> Reports { get; set; } = new();
    }
}