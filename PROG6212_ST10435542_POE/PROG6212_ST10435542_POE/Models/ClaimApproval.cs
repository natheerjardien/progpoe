using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_ST10435542_POE.Models
{
    public class ClaimApproval
    {
        [Key]
        public int ApprovalID { get; set; }
        public int ClaimID { get; set; }

        [Required]
        public int ApproverUserID { get; set; } // ID of the user who approved/rejected
        public ApproverRoleEnum ApproverRole { get; set; } // Role of the approver (ProgrammeCoordinator or AcademicManager)
        public ClaimApprovalStatusEnum ApprovalStatus { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string? Comments { get; set; }

        public virtual MonthlyClaim MonthlyClaim { get; set; }
        [ForeignKey("ApproverUserID")]
        public User Approver { get; set; }
    }
}
