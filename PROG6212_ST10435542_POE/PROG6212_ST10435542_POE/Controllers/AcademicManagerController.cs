using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;

namespace PROG6212_ST10435542_POE.Controllers
{
    // As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing manager-related actions such as adding approval settings, view pending claims, view processe claims, and review claims made by lecturer
    // Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    public class AcademicManagerController : Controller // controller manages academic manager actions such as pending claims, approvals, rejections
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        // According to FullstackPrep (2025), the IWebHostEnvironment interface provides information about the web hosting environment an application is running in, such as the content root path and environment name.
        public AcademicManagerController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        // shows all pending claims for review
        public async Task<IActionResult> PendingClaims()
        {
            ViewBag.StatusMessage = HttpContext.Session.GetString("ManagerStatus");

            var pendingClaims = await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatusEnum.PendingManagerApproval)
                .Select(c => new PendingClaimViewModel
                {
                    ClaimID = c.ClaimID,
                    LecturerName = $"{c.Lecturer.FirstName} {c.Lecturer.LastName}",
                    SubmissionDate = c.SubmissionDate,
                    ClaimPeriod = c.ClaimPeriod,
                    TotalAmount = c.TotalAmount,
                    Status = c.Status,
                    FilePath = c.SupportingDocumentPath
                })
                .ToListAsync();

            return View(pendingClaims); // returns the view populated with pending claims
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var claim = await _context.MonthlyClaims.FindAsync(id);
                if (claim == null) return Json(new { success = false, message = "Claim not found." });

                claim.Status = ClaimStatusEnum.ApprovedByManager;
                claim.ManagerApproved = true;
                claim.ManagerNotes = "Final Approval by Academic Manager";

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("ManagerStatus", $"Successfully Approved Claim #{id}");

                return Json(new { success = true, message = $"Claim #{id} approved." }); // returns success response in json
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var claim = await _context.MonthlyClaims.FindAsync(id);
                if (claim == null) return Json(new { success = false, message = "Claim not found." });

                claim.Status = ClaimStatusEnum.Rejected;
                claim.ManagerApproved = false;
                claim.ManagerNotes = "Rejected by Academic Manager";

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("ManagerStatus", $"Rejected Claim #{id}");

                return Json(new { success = true, message = $"Claim #{id} rejected." }); // returns success message for rejection
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // shows all processed claims
        public async Task<IActionResult> ProcessedClaims()
        {
            var processedClaims = await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatusEnum.ApprovedByManager || c.Status == ClaimStatusEnum.Rejected)
                .OrderByDescending(c => c.SubmissionDate)
                .Select(c => new ProcessedClaimViewModel
                {
                    ClaimID = c.ClaimID,
                    LecturerName = $"{c.Lecturer.FirstName} {c.Lecturer.LastName}",
                    SubmissionDate = c.SubmissionDate,
                    ClaimPeriod = c.ClaimPeriod,
                    TotalAmount = c.TotalAmount,
                    FinalStatus = c.Status,
                    ProcessedDate = DateTime.Now
                })
                .ToListAsync();

            return View(processedClaims);
        }

        // to reveiw a specific claim
        [HttpGet]
        public async Task<IActionResult> ReviewClaim(int id)
        {
            var claim = await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.ClaimID == id);

            if (claim == null)
            {
                return NotFound();
            }

            var model = new ClaimDetailsViewModel
            {
                ClaimID = claim.ClaimID,
                LecturerName = $"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}",
                SubmissionDate = claim.SubmissionDate,
                ClaimPeriod = claim.ClaimPeriod,
                TotalAmount = claim.TotalAmount,
                TotalHoursWorked = claim.TotalHoursWorked,
                Status = claim.Status,
                Notes = claim.ManagerNotes ?? claim.CoordinatorNotes,
                Documents = new List<SupportingDocumentViewModel>()
            };

            if (!string.IsNullOrEmpty(claim.SupportingDocumentPath))
            {
                model.Documents.Add(new SupportingDocumentViewModel
                {
                    FileName = !string.IsNullOrEmpty(claim.OriginalFileName) ? claim.OriginalFileName : "Document",
                    FileUrl = Url.Action("Download", "File", new { id = claim.ClaimID })
                });
            }

            return View(model);
        }
    }
}

/* References:

IIEVC School of Computer Science, 2025. CLDV6212 Building a Modern Web App with Azure Table Storage & ASP.NET Core MVC - Part 1. [video online] 
Available at: <https://youtu.be/Txp7VYUMBGQ?si=5sD7TV0vS90-pPHY>
[Accessed 14 September 2025].

W3Schools, 2025. C# Files. [online] 
Available at: <https://www.w3schools.com/cs/cs_files.php>
[Accessed 10 October 2025].

C-SharpCorner, 2017. How to Read and Write JSON Files in C#. [online] 
Available at: <https://www.c-sharpcorner.com/article/how-to-read-and-write-json-files-in-c-sharp/>
[Accessed 14 October 2025].

FullstackPrep, 2025. Working with IWebHostEnvironment in ASP.NET Core. [online] 
Available at: <https://www.fullstackprep.dev/articles/webd/aspnet/IWebHostEnvironment-in-aspnet-core>
[Accessed 11 October 2025].

*/
