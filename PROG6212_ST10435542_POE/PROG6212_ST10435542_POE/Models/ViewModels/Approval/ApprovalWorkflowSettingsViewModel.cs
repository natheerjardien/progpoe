using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.Approval
{
    public class ApprovalWorkflowSettingsViewModel
    {
        [Display(Name = "Maximum Hours per Claim")]
        public decimal MaxHoursPerClaim { get; set; }

        [Display(Name = "Default Hourly Rate")]
        public decimal DefaultHourlyRate { get; set; }

        [Display(Name = "Maximum Total Amount per Claim")]
        public decimal MaxTotalAmountPerClaim { get; set; }

        [Display(Name = "Require Supporting Documents for all Claims")]
        public bool RequireSupportingDocuments { get; set; }

        [Display(Name = "Allowed File Types (comma-separated)")]
        public string AllowedFileTypes { get; set; } = ".pdf,.docx,.xlsx";
    }
}
