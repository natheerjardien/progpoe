using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.Lecturer
{
    public class SubmitClaimViewModel
    {
        [Display(Name = "Claim Period")]
        [Required]
        public DateTime ClaimPeriod { get; set; }

        [Display(Name = "Your Hourly Rate")]
        [Range(0, double.MaxValue, ErrorMessage = "Hourly rate must be 0 or higher")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Hours Worked for Period")]
        [Range(0, double.MaxValue, ErrorMessage = "Hours must be between 0 or higher")]
        public decimal TotalHoursWorked { get; set; }

        public decimal TotalAmount { get; set; }

        [Display(Name = "Additional Notes")]
        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [Display(Name = "Supporting Document")]
        [Required(ErrorMessage = "Please upload a supporting document")]
        public IFormFile? SupportingDocument { get; set; }


        public List<ClaimDetailInputViewModel>? ClaimDetails { get; set; }
    }

    public class ClaimDetailInputViewModel
    {
        [Display(Name = "Date Worked")]
        [Required]
        public DateTime DateWorked { get; set; }

        [Display(Name = "Hours Worked")]
        [Required]
        [Range(0.1, 24, ErrorMessage = "Hours worked must be positive")]
        public decimal HoursWorked { get; set; }

        [Display(Name = "Task Description")]
        [Required]
        [MaxLength(250)]
        public string TaskDescription { get; set; }
    }
}
