using PROG6212_ST10435542_POE.Models.Enums;

namespace PROG6212_ST10435542_POE.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // personal details
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public UserRoleEnum Role { get; set; } // controls access level

        public Lecturer? LecturerProfile { get; set; }
        public ProgrammeCoordinator? CoordinatorProfile { get; set; }
        public AcademicManager? AcademicManagerProfile { get; set; }
        public ICollection<ClaimApproval>? InitiatedClaimApprovals { get; set; }
    }
}
