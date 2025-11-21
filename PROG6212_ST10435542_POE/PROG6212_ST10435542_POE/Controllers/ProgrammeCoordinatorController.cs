using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;

namespace PROG6212_ST10435542_POE.Controllers
{
// As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing coordinator-related actions such as adding approval settings, view pending claims, view processe claims, and review claims made by lecturer
// Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    [Authorize(Roles = "ProgrammeCoordinator")]
    public class ProgrammeCoordinatorController : Controller
    {
        private readonly IWebHostEnvironment _env; // allows access to uploads folder
        private readonly ApplicationDbContext _context;

        // According to FullstackPrep (2025), the constructor initializes the controller with the web host environment to access file paths.
        public ProgrammeCoordinatorController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        public async Task<IActionResult> PendingClaims()
        {
            ViewBag.LastActionMessage = HttpContext.Session.GetString("LastAction");

            var pendingClaims = await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatusEnum.PendingCoordinatorApproval)
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

            return View(pendingClaims); // returns view with pending claims
        }

        [HttpPost]
        public async Task<IActionResult> VerifyClaim(int id)
        {
            try
            {
                var claim = await _context.MonthlyClaims.FindAsync(id);

                if (claim == null)
                {
                    return Json(new { success = false, message = "Claim not found." });
                }

                claim.Status = ClaimStatusEnum.PendingManagerApproval;
                claim.CoordinatorApproved = true;
                claim.CoordinatorNotes = "Verified by Coordinator";

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("LastAction", $"Verified Claim #{id} for {claim.TotalAmount:C} at {DateTime.Now.ToShortTimeString()}");

                return Json(new { success = true, message = $"Claim #{id} verified and forwarded to Manager." }); // returns success response
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
        {
            try
            {
                var claim = await _context.MonthlyClaims.FindAsync(id);

                if (claim == null)
                {
                    return Json(new { success = false, message = "Claim not found." });
                }

                claim.Status = ClaimStatusEnum.Rejected;
                claim.CoordinatorApproved = false;
                claim.CoordinatorNotes = "Rejected by Coordinator";

                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("LastAction", $"Rejected Claim #{id} at {DateTime.Now.ToShortTimeString()}");

                return Json(new { success = true, message = $"Claim #{id} rejected." }); // returna success response
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ProcessedClaims()
        {
            var processedClaims = await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .Where(c => c.Status != ClaimStatusEnum.PendingCoordinatorApproval && c.Status != ClaimStatusEnum.Submitted)
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

        [HttpGet]
        public async Task<IActionResult> ReviewClaim(int id)
        {
            var claim = await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.ClaimID == id);

            if (claim == null)
            {
                return NotFound("Claim not found.");
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
                Documents = new List<SupportingDocumentViewModel>()
            };

            if (!string.IsNullOrEmpty(claim.SupportingDocumentPath))
            {
                model.Documents.Add(new SupportingDocumentViewModel
                {
                    FileName = claim.OriginalFileName,
                    FileUrl = Url.Action("Download", "File", new { id = claim.ClaimID }),
                    Size = 0
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
