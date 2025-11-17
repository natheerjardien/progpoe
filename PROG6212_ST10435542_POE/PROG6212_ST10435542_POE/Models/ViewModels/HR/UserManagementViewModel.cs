using PROG6212_ST10435542_POE.Models.Enums;

namespace PROG6212_ST10435542_POE.Models.ViewModels.HR
{
    public class UserManagementViewModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRoleEnum Role { get; set; }
        public bool HasLecturerProfile { get; set; }
        public int? LecturerProfileID { get; set; }
    }
}
