namespace PROG6212_ST10435542_POE.Models.ViewModels.HR
{
    public class ReportsDisplayViewModel
    {
        public string ReportTitle { get; set; } = "Generated Report";
        public string? ReportDescription { get; set; }

        public List<string> ReportHeaders { get; set; } = new List<string>();
        public List<List<string>> ReportData { get; set; } = new List<List<string>>();
    }
}
