using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_ST10435542_POE.Models
{
    public class MonthlyClaim
    {
        [Key]
        public int ClaimID { get; set; }
        public string LecturerID { get; set; }
        public DateTime ClaimPeriod { get; set; } // better representation of the date of claim
        public decimal TotalHoursWorked { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SubmissionDate { get; set; }

        public ClaimStatusEnum Status { get; set; } // status of the claim

        [ForeignKey("LecturerID")]
        public ApplicationUser Lecturer { get; set; }

        public string SupportingDocumentPath { get; set; }
        public string OriginalFileName { get; set; }
        public long FileSize { get; set; }

        public bool CoordinatorApproved { get; set; }
        public string CoordinatorNotes { get; set; }
        public bool ManagerApproved { get; set; }
        public string ManagerNotes { get; set; }

        //public Lecturer Lecturer { get; set; }
        //public ICollection<ClaimDetail>? ClaimDetails { get; set; }
        //public ICollection<SupportingDocument>? SupportingDocuments { get; set; }
        //public ICollection<ClaimApproval>? ClaimApprovals { get; set; }
    }
}
