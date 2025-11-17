using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PROG6212_ST10435542_POE.Models.ViewModels.Approval
{
    public class ClaimDetailsViewModel
    {
        public int ClaimID { get; set; }

        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [Display(Name = "Claim Period")]
        public DateTime ClaimPeriod { get; set; }

        [Display(Name = "Total Hours Worked")]
        public decimal TotalHoursWorked { get; set; }

        [Display(Name = "Total Amount Claimed")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Current Status")]
        public ClaimStatusEnum Status { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Collection of detailed work entries
        public List<ClaimDetailItemViewModel>? ClaimDetails { get; set; }

        // Collection of supporting documents data
        public List<SupportingDocumentViewModel>? Documents { get; set; }

        // Collection of approval history records
        public List<ClaimApprovalItemViewModel>? ApprovalHistory { get; set; }

        [Display(Name = "Your Comments (optional)")]
        public string? ManagerComments { get; set; } // For Academic Manager
        public string? CoordinatorComments { get; set; } // For Programme Coordinator
    }

    public class ClaimDetailItemViewModel
    {
        public int ClaimDetailID { get; set; }
        [Display(Name = "Date Worked")]
        public DateTime DateWorked { get; set; }
        [Display(Name = "Hours")]
        public decimal HoursWorked { get; set; }
        [Display(Name = "Description")]
        public string TaskDescription { get; set; }
    }

    public class SupportingDocumentViewModel
    {
        public int DocumentID { get; set; }
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        public string FileUrl { get; set; } // Url to access the stored file
        public long Size { get; set; }
        public DateTimeOffset? LastModified { get; set; }

        [Display(Name = "Size")]
        public string DisplaySize
        {
            get
            {
                if (Size >= 1024 * 1024)
                {
                    return $"{Size / (1024.0 * 1024.0):F2} MB";
                }
                if (Size >= 1024)
                {
                    return $"{Size / 1024.0:F2} KB";
                }
                return $"{Size} Bytes";
            }
        }
    }

    public class ClaimApprovalItemViewModel
    {
        public int ApprovalID { get; set; }
        [Display(Name = "Approver")]
        public string ApproverName { get; set; }
        [Display(Name = "Role")]
        public ApproverRoleEnum ApproverRole { get; set; }
        [Display(Name = "Action")]
        public ClaimApprovalStatusEnum ApprovalStatus { get; set; }
        [Display(Name = "Date")]
        public DateTime ApprovalDate { get; set; }
        [Display(Name = "Comments")]
        public string? Comments { get; set; }
    }
}
