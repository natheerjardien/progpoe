using Microsoft.AspNetCore.Identity;

namespace PROG6212_ST10435542_POE.Models
{
    public class User : IdentityUser
    {
        //public string UserId { get; set; }
        // personal details
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }

        public Lecturer? LecturerProfile { get; set; }
        public ProgrammeCoordinator? CoordinatorProfile { get; set; }
        public AcademicManager? AcademicManagerProfile { get; set; }
        public ICollection<ClaimApproval>? InitiatedClaimApprovals { get; set; }
    }
}
