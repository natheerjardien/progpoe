using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_ST10435542_POE.Models;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;
using PROG6212_ST10435542_POE.Models.ViewModels.Lecturer;
using PROG6212_ST10435542_POE.Services;
using System.Text.Json;

namespace PROG6212_ST10435542_POE.Controllers
{
    // As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing lecturer-related actions such as adding adding new claims, viewing claim details, tracking claim status, and managing personal profile
    // Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    [Authorize(Roles = "Lecturer")] // restricts access to users with Lecturer role
    public class LecturerController : Controller
    {
// According to FullstackPrep (2025), IWebHostEnvironment provides information about the web hosting environment an application is running in.
        private readonly IFileStorageService _fileStorageService; // handles the encrypted file uploads
        private readonly IWebHostEnvironment _env; // gives access to the uploads folder
        private readonly ApplicationDbContext _context; // database context for data operations
        private readonly UserManager<ApplicationUser> _userManager; // to get logged-in user info

        // defines paths for uploads folder and claims json file
        private string UploadsFolder => Path.Combine(_env.ContentRootPath, "uploads"); // folder to store uploaded files
        private string ClaimsJsonFile => Path.Combine(UploadsFolder, "claims.json"); // JSON file to store claims data


        public LecturerController(IFileStorageService fileStorageService, IWebHostEnvironment env, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _fileStorageService = fileStorageService;
            _env = env;
            _context = context;
            _userManager = userManager;

            //Directory.CreateDirectory(UploadsFolder); // makes sure the folder exists
        }

        // displays the form for lecturers to submit a new claim
        [HttpGet]
        public async Task<IActionResult> SubmitClaim()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new SubmitClaimViewModel
            {
                HourlyRate = user.HourlyRate,
                ClaimPeriod = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim (SubmitClaimViewModel model, CancellationToken ct) // handles the form submission for new claims
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            model.HourlyRate = user.HourlyRate;

            if (model.TotalHoursWorked > 180)
            {
                ModelState.AddModelError("TotalHoursWorked", "You cannot claim for more than 180 hours in a single month.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.TotalAmount = model.TotalHoursWorked * model.HourlyRate; // calculates the total amount

            string? encryptedFileName = null;
            string? originalFileName = null;
            long fileSize = 0;

            if (model.SupportingDocument != null)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var ext = Path.GetExtension(model.SupportingDocument.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("SupportingDocument", "Invalid file type. Only PDF, DOCX, XLSX allowed.");
                    return View(model);
                }

                if (model.SupportingDocument.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("SupportingDocument", "File too large. Max size 5 MB.");
                    return View(model);
                }

                try
                {
                    string tempId = Guid.NewGuid().ToString();
                    var result = await _fileStorageService.SaveEncryptedAsync(model.SupportingDocument, tempId, ct);
                    encryptedFileName = result.EncryptedFileName;
                    originalFileName = result.OriginalFileName;
                    fileSize = result.Size;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"File upload failed: {ex.Message}");
                    return View(model);
                }
            }

            var newClaim = new MonthlyClaim // saves claim to DB
            {
                LecturerID = user.Id,
                SubmissionDate = DateTime.Now,
                ClaimPeriod = model.ClaimPeriod,
                TotalHoursWorked = model.TotalHoursWorked,
                TotalAmount = model.TotalAmount,
                Status = ClaimStatusEnum.PendingCoordinatorApproval,
                OriginalFileName = originalFileName ?? "",
                SupportingDocumentPath = encryptedFileName ?? "",
                FileSize = fileSize,
                CoordinatorApproved = false,
                ManagerApproved = false,
                CoordinatorNotes = "",
                ManagerNotes = ""
            };

            _context.MonthlyClaims.Add(newClaim);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim submitted successfully!";
            return RedirectToAction("ClaimStatusTracker");
        }

        // displays details for a specific claim submitted by the lecturer
        [HttpGet("Lecturer/ClaimDetails/{id}")] // routes to view the detailed info about a specific claim
        public async Task<IActionResult> ClaimDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var claim = await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.ClaimID == id && c.LecturerID == user.Id);

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
                Documents = new List<SupportingDocumentViewModel>()
            };

            if (!string.IsNullOrEmpty(claim.SupportingDocumentPath))
            {
                model.Documents.Add(new SupportingDocumentViewModel
                {
                    FileName = claim.OriginalFileName,
                    FileUrl = Url.Action("Download", "File", new { id = claim.ClaimID }),
                    Size = claim.FileSize
                });
            }

            model.ApprovalHistory = new List<ClaimApprovalItemViewModel> // hard coded for now. Will chnage it later
            {
                new ClaimApprovalItemViewModel
                {
                    ApproverName = "Coordinator",
                    ApproverRole = ApproverRoleEnum.ProgrammeCoordinator,
                    ApprovalStatus = claim.CoordinatorApproved ? ClaimApprovalStatusEnum.Approved : ClaimApprovalStatusEnum.Pending,
                    ApprovalDate = DateTime.Now,
                    Comments = claim.CoordinatorNotes ?? "Pending Review"
                },
                new ClaimApprovalItemViewModel
                {
                    ApproverName = "Manager",
                    ApproverRole = ApproverRoleEnum.AcademicManager,
                    ApprovalStatus = claim.ManagerApproved ? ClaimApprovalStatusEnum.Approved : ClaimApprovalStatusEnum.Pending,
                    ApprovalDate = DateTime.Now,
                    Comments = claim.ManagerNotes ?? "Pending Review"
                }
            };

            return View(model);
        }

        // displays a list of all claims submitted by the lecturer
        public async Task<IActionResult> ClaimStatusTracker() // reads from DB
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var claims = await _context.MonthlyClaims
                .Where(c => c.LecturerID == user.Id)
                .OrderByDescending(c => c.SubmissionDate)
                .Select(c => new ClaimStatusViewModel
                {
                    ClaimID = c.ClaimID,
                    ClaimPeriod = c.ClaimPeriod,
                    SubmissionDate = c.SubmissionDate,
                    TotalAmount = c.TotalAmount,
                    Status = c.Status,
                    LastUpdate = c.SubmissionDate
                })
                .ToListAsync();

            return View(claims);
        }

        // displays the lecturer's personal and financial profile
        public async Task<IActionResult> LecturerProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new LecturerProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                HourlyRate = user.HourlyRate
            };

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
