using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.Approval
{
    public class PendingClaimViewModel
    {
        public int ClaimID { get; set; }

        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [Display(Name = "Claim Period")]
        public DateTime ClaimPeriod { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Status")]
        public ClaimStatusEnum Status { get; set; }

        [Display(Name = "Supporting Documents")]
        public string FilePath { get; set; }
    }
}
