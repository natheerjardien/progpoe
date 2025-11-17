using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;
using PROG6212_ST10435542_POE.Models.ViewModels.Lecturer;
using PROG6212_ST10435542_POE.Services;
using System.Text.Json;

namespace PROG6212_ST10435542_POE.Controllers
{
// As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing lecturer-related actions such as adding adding new claims, viewing claim details, tracking claim status, and managing personal profile
// Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    public class LecturerController : Controller
    {
// According to FullstackPrep (2025), IWebHostEnvironment provides information about the web hosting environment an application is running in.
        private readonly IFileStorageService _fileStorageService; // handles the encrypted file uploads
        private readonly IWebHostEnvironment _env; // gives access to the uploads folder

        // defines paths for uploads folder and claims json file
        private string UploadsFolder => Path.Combine(_env.ContentRootPath, "uploads"); // folder to store uploaded files
        private string ClaimsJsonFile => Path.Combine(UploadsFolder, "claims.json"); // JSON file to store claims data


        public LecturerController(IFileStorageService fileStorageService, IWebHostEnvironment env)
        {
            _fileStorageService = fileStorageService;
            _env = env;

            Directory.CreateDirectory(UploadsFolder); // makes sure the folder exists
        }

        // displays the form for lecturers to submit a new claim
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            var model = new SubmitClaimViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim (SubmitClaimViewModel model, CancellationToken ct) // handles the form submission for new claims
        {
            if (!ModelState.IsValid)
            {
                return View(model); // returns the form with validation messages
            }

            model.TotalAmount = model.TotalHoursWorked * model.HourlyRate; // calculates total amount

            List<Dictionary<string, object>> existingClaims = new(); // to hold existing claims

// According to W3Schools (2025), System.IO.File.Exists checks if a file exists at the specified path.
            if (System.IO.File.Exists(ClaimsJsonFile)) // checks if claims file already exists
            {
// According to W3Schools (2025), System.IO.File.ReadAllText reads the contents of a file into a string.
                var json = System.IO.File.ReadAllText(ClaimsJsonFile); // reads the JSON file as a string
// According to C-SharpCorner (2017), JsonSerializer.Deserialize converts JSON text into .NET objects.
                existingClaims = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json) ?? new(); // deserializes json into a list of claims
            }
            else
            {
                // Ensure uploads directory exists
                var uploadDir = Path.GetDirectoryName(ClaimsJsonFile);// gets the directory path for json file
                if (!Directory.Exists(uploadDir)) // ensures directory exists before creating the file
                {
                    Directory.CreateDirectory(uploadDir!); // creates directory if missing
                }

                // Initialize an empty claims file
                System.IO.File.WriteAllText(ClaimsJsonFile, "[]");
            }

            int newClaimId = existingClaims.Count > 0 ? existingClaims.Max(c => ((JsonElement)c["ClaimID"]).GetInt32()) + 1 : 1; // generates new claimID incremnentally
            string? encryptedFileName = null;
            string? originalFileName = null;

            if (model.SupportingDocument != null) // validates the uploaded file
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" }; // allowed file types for security
                var ext = Path.GetExtension(model.SupportingDocument.FileName).ToLowerInvariant(); // gets file extension in lowercase

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("", "Invalid file type. Only PDF, DOCX, XLSX allowed."); // adds error if invalid file
                    return View(model); // reloads form with error
                }

                if (model.SupportingDocument.Length > 5 * 1024 * 1024) // checks if file exceeds 5mb
                {
                    ModelState.AddModelError("", "File too large. Max size 5 MB.");
                    return View(model);
                }

                try
                {
                    var result = await _fileStorageService.SaveEncryptedAsync(model.SupportingDocument, newClaimId.ToString(), ct); // calls custom service to encrypt and save the file
                    encryptedFileName = result.EncryptedFileName; // stores encrypted filename returned by service
                    originalFileName = result.OriginalFileName; // stores original filename

                    ViewBag.FileMessage = $"File '{result.OriginalFileName}' uploaded successfully";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"File upload failed: {ex.Message}");
                    return View(model);
                }
            }

            var newClaim = new Dictionary<string, object> // creates a new claim record to save in json
            {
                ["ClaimID"] = newClaimId,
                ["LecturerName"] = "Natheer Jardien", // static lecturer name for now
                ["EncryptedFileName"] = encryptedFileName ?? "",
                ["OriginalFileName"] = originalFileName ?? model.SupportingDocument?.FileName ?? "",
                ["SubmissionDate"] = DateTime.Now,
                ["ClaimPeriod"] = model.ClaimPeriod,
                ["HourlyRate"] = model.HourlyRate,
                ["TotalHoursWorked"] = model.TotalHoursWorked,
                ["TotalAmount"] = model.TotalAmount,
                ["Notes"] = model.Notes ?? "",
                ["ClaimDetails"] = model.ClaimDetails ?? new List<ClaimDetailInputViewModel>(),
                ["Status"] = ClaimStatusEnum.PendingCoordinatorApproval.ToString(), // sets initial status to pending approval
                ["LastUpdate"] = DateTime.Now
            };

            existingClaims.Add(newClaim); // adds the new claim to existing list
            System.IO.File.WriteAllText(ClaimsJsonFile, JsonSerializer.Serialize(existingClaims)); // saves updated claim list back to json file

            //TempData["SuccessMessage"] = "Claim submiited succesfully ;)";

            return RedirectToAction("ClaimStatusTracker");
        }

        // displays details for a specific claim submitted by the lecturer
        [HttpGet("Lecturer/ClaimDetails/{id}")] // routes to view the detailed info about a specific claim
        public IActionResult ClaimDetails(int id)
        {
            if (!System.IO.File.Exists(ClaimsJsonFile))  // checks if claims file exists
            {
                return NotFound();
            }

            var json = System.IO.File.ReadAllText(ClaimsJsonFile); // reads the json data
            using var doc = JsonDocument.Parse(json); // parses json into a document

            if (doc.RootElement.ValueKind != JsonValueKind.Array) // ensures valid json array format
            {
                return NotFound();
            }

            foreach (var c in doc.RootElement.EnumerateArray()) // iterates through each claim in the array
            {
                if (!c.TryGetProperty("ClaimID", out var idProp) || idProp.GetInt32() != id) // matches claim by id
                {
                    continue; // skip if not matching
                }

                string lecturerName = c.TryGetProperty("LecturerName", out var lnProp) ? (lnProp.GetString() ?? "") : ""; // retrieves lecturer name
                DateTime submissionDate = c.TryGetProperty("SubmissionDate", out var sdProp) && DateTime.TryParse(sdProp.GetString(), out var sdVal) ? sdVal : DateTime.MinValue; // parses submission date
                DateTime claimPeriod = c.TryGetProperty("ClaimPeriod", out var cpProp) && DateTime.TryParse(cpProp.GetString(), out var cpVal) ? cpVal : DateTime.MinValue; // parses claim period
                decimal totalHoursWorked = c.TryGetProperty("TotalHoursWorked", out var thwProp) ? thwProp.GetDecimal() : 0m; // gets total hours
                decimal hourlyRate = c.TryGetProperty("HourlyRate", out var hrProp) ? hrProp.GetDecimal() : 0m; // gets hourly rate
                decimal totalAmount = c.TryGetProperty("TotalAmount", out var taProp) ? taProp.GetDecimal() : (totalHoursWorked * hourlyRate); // calculates total if missing

                string statusString = c.TryGetProperty("Status", out var statusProp) ? (statusProp.GetString() ?? "") : ""; // gets claim status string
                ClaimStatusEnum status = Enum.TryParse<ClaimStatusEnum>(statusString, out var statusVal) ? statusVal : ClaimStatusEnum.Submitted; // converts string to enum

                string originalFileName = c.TryGetProperty("OriginalFileName", out var ofnProp) ? (ofnProp.GetString() ?? "") : ""; // retrieves original filename
                string encryptedFileName = c.TryGetProperty("EncryptedFileName", out var efnProp) ? (efnProp.GetString() ?? "") : ""; // retrieves encrypted filename

                var model = new ClaimDetailsViewModel // populates viewmodel with claim data for display
                {
                    ClaimID = id,
                    LecturerName = lecturerName,
                    SubmissionDate = submissionDate,
                    ClaimPeriod = claimPeriod,
                    TotalAmount = totalAmount,
                    TotalHoursWorked = totalHoursWorked,
                    Status = status,
                    Documents = new List<SupportingDocumentViewModel>()
                };

                if (!string.IsNullOrEmpty(encryptedFileName)) // checks if document exists
                {
                    model.Documents.Add(new SupportingDocumentViewModel
                    {
                        FileName = !string.IsNullOrEmpty(originalFileName) ? originalFileName : encryptedFileName,
                        FileUrl = Url.Action("Download", "File", new { id }) ?? string.Empty, // link to download file
                        Size = 0
                    });
                }

                // Simple approval history (uses your existing view model types)
                model.ApprovalHistory = new List<ClaimApprovalItemViewModel>
                {
                    new ClaimApprovalItemViewModel
                    {
                        ApprovalID = 1,
                        ApproverName = "System",
                        ApproverRole = ApproverRoleEnum.ProgrammeCoordinator,
                        ApprovalStatus = ClaimApprovalStatusEnum.Pending,
                        ApprovalDate = model.SubmissionDate,
                        Comments = "Claim submitted"
                    }
                };

                return View(model);
            }

            return NotFound();
        }

        // displays a list of all claims submitted by the lecturer
        public IActionResult ClaimStatusTracker()
        {            
            if (!System.IO.File.Exists(ClaimsJsonFile)) // checks if JSON file exists
            {                
                return View(new List<ClaimStatusViewModel>()); // no claims submitted yet
            }

            // reads the stored claims from JSON
            var storedClaims = JsonSerializer.Deserialize<List<dynamic>>(System.IO.File.ReadAllText(ClaimsJsonFile)) ?? new List<dynamic>(); // loads stored claims from json

            // maps to ClaimStatusViewModel
            var claims = storedClaims.Select(c => new ClaimStatusViewModel
            {
                ClaimID = c.GetProperty("ClaimID").GetInt32(),
                ClaimPeriod = DateTime.Parse(c.GetProperty("ClaimPeriod").GetString()!),
                SubmissionDate = DateTime.Parse(c.GetProperty("SubmissionDate").GetString()!),
                TotalAmount = c.GetProperty("TotalAmount").GetDecimal(),
                Status = Enum.TryParse<ClaimStatusEnum>(c.GetProperty("Status").GetString(), out ClaimStatusEnum status) ? status : ClaimStatusEnum.Submitted,
                LastUpdate = DateTime.Parse(c.GetProperty("LastUpdate").GetString()!)
            })
            .OrderByDescending(c => c.SubmissionDate) // orders claims by most recent submission first
            .ToList();

            return View(claims);
        }

        // displays the lecturer's personal and financial profile
        public IActionResult LecturerProfile()
        {
            return View(new LecturerProfileViewModel());
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
