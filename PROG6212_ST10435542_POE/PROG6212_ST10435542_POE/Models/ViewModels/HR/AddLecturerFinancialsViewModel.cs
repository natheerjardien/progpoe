using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.HR
{
    public class AddLecturerFinancialsViewModel
    {
        [Display(Name = "Select User (Lecturer)")]
        public int UserID { get; set; }

        // ViewBag.AvailableLecturerUsers will hold SelectListItems for the dropdown
        public SelectList? AvailableLecturerUsers { get; set; }

        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Bank Details")]
        public string BankDetails { get; set; }

        [Display(Name = "Date Hired")]
        public DateTime? DateHired { get; set; }
    }
}
