using WasteConnect.Models;

namespace WasteConnect.ViewModels
{
    public class CommunityAlertsViewModel
    {
        public List<CommunityAlert> WaterAlerts { get; set; } = new();

        public List<CommunityAlert> ElectricityAlerts { get; set; } = new();

        public List<CommunityAlert> BinCollectionAlerts { get; set; } = new();
    }
}