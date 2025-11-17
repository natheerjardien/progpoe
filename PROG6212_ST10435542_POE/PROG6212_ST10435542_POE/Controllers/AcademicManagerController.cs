using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;
using System.Text.Json;

namespace PROG6212_ST10435542_POE.Controllers
{
    // As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing manager-related actions such as adding approval settings, view pending claims, view processe claims, and review claims made by lecturer
    // Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    public class AcademicManagerController : Controller // controller manages academic manager actions such as pending claims, approvals, rejections
    {
        private readonly IWebHostEnvironment _env;
        private string ClaimsJsonFile => Path.Combine(_env.ContentRootPath, "uploads", "claims.json"); // json file path for claims

// According to FullstackPrep (2025), the IWebHostEnvironment interface provides information about the web hosting environment an application is running in, such as the content root path and environment name.
        public AcademicManagerController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // shows all pending claims for review
        public IActionResult PendingClaims()
        {
// According to W3Schools (2025), the File.Exists method checks if a specified file exists and returns a boolean value.
            if (!System.IO.File.Exists(ClaimsJsonFile)) // checks if claims file exists to avoid exceptions
            {
                return View(new List<PendingClaimViewModel>()); // returns empty list if file missing
            }

            var storedClaims = JsonSerializer.Deserialize<List<dynamic>>(System.IO.File.ReadAllText(ClaimsJsonFile)) ?? new List<dynamic>(); // deserializes claims into a dynamic list

            var model = storedClaims
                .Where(c =>
                {
                    var statusProp = c.GetProperty("Status"); // retrieves status property from claim
                    string statusString = statusProp.ValueKind switch // converts json status to string based on json type
                    {
                        JsonValueKind.String => statusProp.GetString()!,
                        JsonValueKind.Number => statusProp.GetRawText(),
                        _ => ""
                    };
                    return Enum.TryParse<ClaimStatusEnum>(statusString, out var status) && // parses string to enum
                           status == ClaimStatusEnum.PendingManagerApproval; // filters only claims pending manager approval
                })
                .Select(c => new PendingClaimViewModel
                {
                    ClaimID = c.GetProperty("ClaimID").GetInt32(), // maps claim id to viewmodel
                    LecturerName = c.GetProperty("LecturerName").GetString() ?? "", // maps lecturer name with fallback
                    SubmissionDate = DateTime.Parse(c.GetProperty("SubmissionDate").GetString()!), // converts string date to datetime
                    ClaimPeriod = DateTime.Parse(c.GetProperty("ClaimPeriod").GetString()!), // converts claim period to datetime
                    TotalAmount = c.GetProperty("TotalAmount").GetDecimal(), // maps total claim amount
                    Status = ClaimStatusEnum.PendingManagerApproval, // sets status explicitly for viewmodel
                    FilePath = c.GetProperty("EncryptedFileName").GetString() ?? "" // uses encrypted file path for document reference
                })
                .ToList(); // converts filtered enumerable to list for the view

            return View(model); // returns the view populated with pending claims
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            try
            {
                UpdateStatus(id, ClaimStatusEnum.ApprovedByManager); // updates claim status to approved by manager
                return Json(new { success = true, message = $"Claim #{id} approved." }); // returns success response in json
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            try
            {
                UpdateStatus(id, ClaimStatusEnum.Rejected); // updates claim status to rejected
                return Json(new { success = true, message = $"Claim #{id} rejected." }); // returns success message for rejection
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private void UpdateStatus(int id, ClaimStatusEnum status)
        {
            if (!System.IO.File.Exists(ClaimsJsonFile)) return; // exits if claims file does not exist


            var json = System.IO.File.ReadAllText(ClaimsJsonFile); // reads claims file content
            using var doc = JsonDocument.Parse(json); // parses json content into document

            if (doc.RootElement.ValueKind != JsonValueKind.Array) // ensures root element is an array
            {
                return; // exits if structure invalid
            }

            var claims = new List<Dictionary<string, object>>(); // prepares list of dictionaries to store claims

            foreach (var c in doc.RootElement.EnumerateArray()) // iterates through each claim element
            {
                var claim = new Dictionary<string, object>(); // creates dictionary to hold claim properties

                // extracts all claim properties and adds to the dictionary
                if (c.TryGetProperty("ClaimID", out var idProp))
                    claim["ClaimID"] = idProp.GetInt32();
                if (c.TryGetProperty("LecturerName", out var lnProp))
                    claim["LecturerName"] = lnProp.GetString() ?? "";
                if (c.TryGetProperty("SubmissionDate", out var sdProp))
                    claim["SubmissionDate"] = sdProp.GetString() ?? "";
                if (c.TryGetProperty("ClaimPeriod", out var cpProp))
                    claim["ClaimPeriod"] = cpProp.GetString() ?? "";
                if (c.TryGetProperty("HourlyRate", out var hrProp))
                    claim["HourlyRate"] = hrProp.GetDecimal();
                if (c.TryGetProperty("TotalHoursWorked", out var thwProp))
                    claim["TotalHoursWorked"] = thwProp.GetDecimal();
                if (c.TryGetProperty("TotalAmount", out var taProp))
                    claim["TotalAmount"] = taProp.GetDecimal();
                if (c.TryGetProperty("Notes", out var notesProp))
                    claim["Notes"] = notesProp.GetString() ?? "";
                if (c.TryGetProperty("ClaimDetails", out var cdProp))
                    claim["ClaimDetails"] = cdProp.GetRawText();
                if (c.TryGetProperty("EncryptedFileName", out var efnProp))
                    claim["EncryptedFileName"] = efnProp.GetString() ?? "";
                if (c.TryGetProperty("OriginalFileName", out var ofnProp))
                    claim["OriginalFileName"] = ofnProp.GetString() ?? "";

                // updates status and last update for the matching claim
                if (claim.ContainsKey("ClaimID") && Convert.ToInt32(claim["ClaimID"]) == id)
                {
                    claim["Status"] = status.ToString();
                    claim["LastUpdate"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                else
                {
                    // keeps the existing statuses and last update for other claims
                    if (c.TryGetProperty("Status", out var statusProp))
                        claim["Status"] = statusProp.GetString() ?? "";
                    if (c.TryGetProperty("LastUpdate", out var luProp))
                        claim["LastUpdate"] = luProp.GetString() ?? "";
                }

                claims.Add(claim); // adds processed claim to list
            }
// According to W3Schools (2025) and C-SharpCorner (2017), the File.WriteAllText method creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
            System.IO.File.WriteAllText(ClaimsJsonFile, JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true })); // writes updated claims back to json file
        }

        // shows all processed claims
        public IActionResult ProcessedClaims()
        {
            if (!System.IO.File.Exists(ClaimsJsonFile))
            {
                return View(new List<ProcessedClaimViewModel>());
            }

            var json = System.IO.File.ReadAllText(ClaimsJsonFile);
            using var doc = JsonDocument.Parse(json);

            var model = new List<ProcessedClaimViewModel>();

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return View(model);
            }

            foreach (var c in doc.RootElement.EnumerateArray())
            {
                if (!c.TryGetProperty("Status", out var statusProp))
                {
                    continue;
                }

                string statusString = statusProp.ValueKind switch
                {
                    JsonValueKind.String => statusProp.GetString() ?? "",
                    JsonValueKind.Number => statusProp.GetRawText(),
                    _ => ""
                };

                if (!Enum.TryParse<ClaimStatusEnum>(statusString, out var status))
                {
                    continue;
                }

                // Show claims that have been processed by Academic Manager (approved or rejected)
                if (status == ClaimStatusEnum.ApprovedByManager || status == ClaimStatusEnum.Rejected)
                {
                    int claimId = c.TryGetProperty("ClaimID", out var idProp) ? idProp.GetInt32() : 0;
                    string lecturerName = c.TryGetProperty("LecturerName", out var lnProp) ? (lnProp.GetString() ?? "") : "";
                    DateTime submissionDate = c.TryGetProperty("SubmissionDate", out var sdProp) && DateTime.TryParse(sdProp.GetString(), out var sdVal) ? sdVal : DateTime.MinValue;
                    DateTime claimPeriod = c.TryGetProperty("ClaimPeriod", out var cpProp) && DateTime.TryParse(cpProp.GetString(), out var cpVal) ? cpVal : DateTime.MinValue;
                    decimal totalAmount = c.TryGetProperty("TotalAmount", out var taProp) ? taProp.GetDecimal() : 0m;
                    DateTime processedDate = c.TryGetProperty("LastUpdate", out var luProp) && DateTime.TryParse(luProp.GetString(), out var luVal) ? luVal : submissionDate;

                    model.Add(new ProcessedClaimViewModel
                    {
                        ClaimID = claimId,
                        LecturerName = lecturerName,
                        SubmissionDate = submissionDate,
                        ClaimPeriod = claimPeriod,
                        TotalAmount = totalAmount,
                        FinalStatus = status,
                        ProcessedDate = processedDate
                    });
                }
            }

            return View(model.OrderByDescending(c => c.ProcessedDate).ToList());
        }

        // can manage the approval workflow settings
        public IActionResult ApprovalWorkflowSettings()
        {
            var model = new ApprovalWorkflowSettingsViewModel();
            return View(model);
        }

        // to reveiw a specific claim
        public IActionResult ReviewClaim()
        {
            var model = new ClaimDetailsViewModel();
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
