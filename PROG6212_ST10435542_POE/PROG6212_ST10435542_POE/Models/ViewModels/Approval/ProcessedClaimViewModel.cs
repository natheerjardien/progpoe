using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.Approval
{
    public class ProcessedClaimViewModel
    {
        public int ClaimID { get; set; }

        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [Display(Name = "Claim Period")]
        public DateTime ClaimPeriod { get; set; }

        [Display(Name = "Processed Date")]
        public DateTime ProcessedDate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Final Status")]
        public ClaimStatusEnum FinalStatus { get; set; }
    }
}
