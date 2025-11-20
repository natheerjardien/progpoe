using Microsoft.AspNetCore.Mvc.Rendering;
using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.HR
{
    public class GenerateReportViewModel
    {
        [Required]
        [Display(Name = "Report Month")]
        [DataType(DataType.Date)]
        public DateTime ReportMonth { get; set; }
    }

    // Model for displaying the data in the report
    public class InvoiceReportDetail
    {
        public int ClaimId { get; set; }
        public string LecturerName { get; set; }
        public string Email { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
