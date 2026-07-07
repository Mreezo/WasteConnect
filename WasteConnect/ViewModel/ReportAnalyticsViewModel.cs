namespace WasteConnect.ViewModels
{
    public class ReportAnalyticsViewModel
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int AssignedReports { get; set; }
        public int InProgressReports { get; set; }
        public int CleanedReports { get; set; }

        public List<ReportMapPoint> MapPoints { get; set; } = new();
        public List<ReportTrendPoint> WeeklyReports { get; set; } = new();
        public List<ReportTrendPoint> MonthlyReports { get; set; } = new();
    }

    public class ReportMapPoint
    {
        public string Location { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ReportedBy { get; set; }
    }

    public class ReportTrendPoint
    {
        public string Label { get; set; }
        public int Count { get; set; }
    }
}