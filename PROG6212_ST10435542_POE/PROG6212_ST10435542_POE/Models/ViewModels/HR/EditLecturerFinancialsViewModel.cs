using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.HR
{
    public class EditLecturerFinancialsViewModel
    {
        public int UserID { get; set; } // Hidden field for the User

        public int LecturerID { get; set; } // Hidden field for the Lecturer profile

        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; } // Display-only

        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Bank Details")]
        public string BankDetails { get; set; }

        [Display(Name = "Date Hired")]
        public DateTime? DateHired { get; set; }
    }
}
