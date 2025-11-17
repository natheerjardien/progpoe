using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.Lecturer
{
    public class ClaimStatusViewModel
    {
        public int ClaimID { get; set; }

        [Display(Name = "Claim Period")]
        public DateTime ClaimPeriod { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Current Status")]
        public ClaimStatusEnum Status { get; set; }

        [Display(Name = "Last Update")]
        public DateTime? LastUpdate { get; set; } // from the latest ClaimApproval record
    }
}
