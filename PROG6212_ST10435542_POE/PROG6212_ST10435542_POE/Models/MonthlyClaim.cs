using PROG6212_ST10435542_POE.Models.Enums;

namespace PROG6212_ST10435542_POE.Models
{
    public class MonthlyClaim
    {
        public int ClaimID { get; set; }
        public int LecturerID { get; set; }
        public DateTime ClaimPeriod { get; set; } // better representation of the date of claim
        public int TotalHoursWorked { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SubmissionDate { get; set; }

        public ClaimStatusEnum Status { get; set; } // status of the claim

        public Lecturer Lecturer { get; set; }
        public ICollection<ClaimDetail>? ClaimDetails { get; set; }
        public ICollection<SupportingDocument>? SupportingDocuments { get; set; }
        public ICollection<ClaimApproval>? ClaimApprovals { get; set; }
    }
}
