using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.Lecturer
{
    public class LecturerProfileViewModel
    {
        public int UserID { get; set; }
        public int? LecturerID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Your Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Your Bank Details")]
        public string BankDetails { get; set; }

        public DateTime? DateHired { get; set; }
    }
}
