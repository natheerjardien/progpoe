using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models
{
    public class ClaimApproval
    {
        public int ApprovalID { get; set; }
        public int ClaimID { get; set; }

        [Required]
        public int ApproverUserID { get; set; } // ID of the user who approved/rejected
        public ApproverRoleEnum ApproverRole { get; set; } // Role of the approver (ProgrammeCoordinator or AcademicManager)
        public ClaimApprovalStatusEnum ApprovalStatus { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string? Comments { get; set; }

        public MonthlyClaim MonthlyClaim { get; set; }
        public User Approver { get; set; }
    }
}
