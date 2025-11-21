using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.HR
{
    public class UserManagementViewModel
    {
        public string? UserID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "Initial Password (Required for New Users")]
        public string? Password { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        [Required]
        public string SelectedRole { get; set; }

        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
}
