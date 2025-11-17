using Microsoft.AspNetCore.Mvc.Rendering;
using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.HR
{
    public class GenerateReportViewModel
    {
        [Display(Name = "Report Type")]
        public ReportTypesEnum ReportType { get; set; } // using an enum for the predefined report types

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Filter by Lecturer (Optional)")]
        public int? LecturerID { get; set; }

        // ViewBag.Lecturers will hold SelectListItems for the dropdown
        public SelectList? Lecturers { get; set; }
    }
}
