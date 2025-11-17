using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace PROG6212_ST10435542_POE.Controllers
{
// As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing coordinator-related actions such as adding approval settings, view pending claims, view processe claims, and review claims made by lecturer
// Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    public class ProgrammeCoordinatorController : Controller
    {
        private readonly IWebHostEnvironment _env; // allows access to uploads folder
        private string ClaimsJsonFile => Path.Combine(_env.ContentRootPath, "uploads", "claims.json"); // path to claims json file

// According to FullstackPrep (2025), the constructor initializes the controller with the web host environment to access file paths.
        public ProgrammeCoordinatorController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult PendingClaims()
        {
// According to W3Schools (2025) and C-SharpCorner (2017), the code reads and processes a JSON file containing claims, filtering for those pending coordinator approval, and prepares them for display in a view.
            if (!System.IO.File.Exists(ClaimsJsonFile)) // check if claims file exists
            {
                return View(new List<PendingClaimViewModel>());
            }

            var json = System.IO.File.ReadAllText(ClaimsJsonFile); // read json content
            using var doc = JsonDocument.Parse(json); // parse json document

            var model = new List<PendingClaimViewModel>(); // create list to hold pending claims

            if (doc.RootElement.ValueKind != JsonValueKind.Array) // check root element type
            {
                return View(model);
            }

            foreach (var c in doc.RootElement.EnumerateArray()) // iterates through each claim
            {
                if (!c.TryGetProperty("Status", out var statusProp)) // skip if status missing
                {
                    continue;
                }

                string statusString = statusProp.ValueKind switch // gets status as a string
                {
                    JsonValueKind.String => statusProp.GetString() ?? "",
                    JsonValueKind.Number => statusProp.GetRawText(),
                    _ => ""
                };

                if (!Enum.TryParse<ClaimStatusEnum>(statusString, out var status) || status != ClaimStatusEnum.PendingCoordinatorApproval) // only pending coordinator claims
                {
                    continue;
                }

                int claimId = c.TryGetProperty("ClaimID", out var idProp) ? idProp.GetInt32() : 0;  // gets claim id
                string lecturerName = c.TryGetProperty("LecturerName", out var lnProp) ? (lnProp.GetString() ?? "") : ""; // gets lecturer name
                DateTime submissionDate = c.TryGetProperty("SubmissionDate", out var sdProp) && DateTime.TryParse(sdProp.GetString(), out var sdVal) ? sdVal : DateTime.MinValue; // gets submission date
                DateTime claimPeriod = c.TryGetProperty("ClaimPeriod", out var cpProp) && DateTime.TryParse(cpProp.GetString(), out var cpVal) ? cpVal : DateTime.MinValue; // gets claim period
                decimal totalAmount = c.TryGetProperty("TotalAmount", out var taProp) ? taProp.GetDecimal() : 0m; // gets total amount

                string filePath = // gets the file path of the encryoted file
                    (c.TryGetProperty("EncryptedFileName", out var encProp) ? (encProp.GetString() ?? "") : "") != "" ?
                        encProp.GetString()! :
                        (c.TryGetProperty("FilePath", out var fpProp) ? (fpProp.GetString() ?? "") : "");

                model.Add(new PendingClaimViewModel // adds claim to model list
                {
                    ClaimID = claimId,
                    LecturerName = lecturerName,
                    SubmissionDate = submissionDate,
                    ClaimPeriod = claimPeriod,
                    TotalAmount = totalAmount,
                    Status = ClaimStatusEnum.PendingCoordinatorApproval,
                    FilePath = filePath
                });
            }

            return View(model); // returns view with pending claims
        }

        [HttpPost]
        public IActionResult VerifyClaim(int id)
        {
            try
            {
                UpdateClaimStatus(id, ClaimStatusEnum.PendingManagerApproval); // updates claim status to pending manager approval
                return Json(new { success = true, message = $"Claim #{id} verified and forwarded to Manager." }); // returns success response
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            try
            {
                UpdateClaimStatus(id, ClaimStatusEnum.Rejected); // updates claim status to rejected
                return Json(new { success = true, message = $"Claim #{id} rejected." }); // returna success response
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private void UpdateClaimStatus(int id, ClaimStatusEnum newStatus)
        {
            if (!System.IO.File.Exists(ClaimsJsonFile)) // checks if claims file exists
            {
                return;
            }                

            var jsonText = System.IO.File.ReadAllText(ClaimsJsonFile); // reads file content
            var claims = JsonSerializer.Deserialize<List<JsonElement>>(jsonText) ?? new List<JsonElement>(); // deserializes into list of json elements

            var claimDicts = claims.Select(c => // converts to modifiable dictionary
                JsonSerializer.Deserialize<Dictionary<string, object>>(c.GetRawText())!).ToList();

            var claim = claimDicts.FirstOrDefault(c => // finds claim with matching id
                c.TryGetValue("ClaimID", out var idObj) &&
                int.TryParse(idObj?.ToString(), out var idVal) &&
                idVal == id);

            if (claim == null) // exit if not found
            {
                return;
            }

            claim["Status"] = newStatus.ToString(); // updates status
            claim["LastUpdate"] = DateTime.Now; // updates last update timestamp

            var approvalRecord = new Dictionary<string, object> // creates approval history record
            {
                ["Role"] = "Programme Coordinator",
                ["Decision"] = newStatus.ToString(),
                ["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["Comments"] = newStatus switch
                {
                    ClaimStatusEnum.PendingManagerApproval => "Approved by Programme Coordinator",
                    ClaimStatusEnum.Rejected => "Rejected by Programme Coordinator",
                    _ => "Status updated by Programme Coordinator"
                }
            };

            List<Dictionary<string, object>> history;  // prepares history list
            if (claim.TryGetValue("ApprovalHistory", out var historyObj) && historyObj != null) // checks existing history
            {
                try
                {
                    var historyJson = historyObj.ToString(); // converts to string
                    history = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(historyJson) ?? new(); // deserializes existing history
                }
                catch
                {
                    history = new List<Dictionary<string, object>>();
                }
            }
            else
            {
                history = new List<Dictionary<string, object>>();
            }

            history.Add(approvalRecord); // adds new record
            claim["ApprovalHistory"] = history; // updates claim dictionary

            System.IO.File.WriteAllText(ClaimsJsonFile, // writes all changes back
                JsonSerializer.Serialize(claimDicts, new JsonSerializerOptions { WriteIndented = true }));
        }


        public IActionResult ViewDocument(int id)
        {
            return RedirectToAction("Download", "File", new { id }); // redirects to file download
        }

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

                // Show claims that have been processed (not pending coordinator approval)
                if (status != ClaimStatusEnum.PendingCoordinatorApproval && status != ClaimStatusEnum.Submitted)
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

        [HttpGet]
        public IActionResult ReviewClaim(int id)
        {
            if (!System.IO.File.Exists(ClaimsJsonFile)) // checks if claims file exists
            {
                return NotFound("Claims file not found.");
            }

            var jsonText = System.IO.File.ReadAllText(ClaimsJsonFile); // reads json content
            using var doc = JsonDocument.Parse(jsonText); // parses json document

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return NotFound("No claims found.");
            }

            var claimElement = doc.RootElement
                .EnumerateArray()
                .FirstOrDefault(c => c.TryGetProperty("ClaimID", out var idProp) && idProp.GetInt32() == id); // finds claim by id

            if (claimElement.ValueKind == JsonValueKind.Undefined) // checks if found
            {
                return NotFound("Claim not found.");
            }

            string lecturerName = claimElement.TryGetProperty("LecturerName", out var lnProp) ? (lnProp.GetString() ?? "") : ""; // gets lecturer name
            DateTime submissionDate = claimElement.TryGetProperty("SubmissionDate", out var sdProp) && DateTime.TryParse(sdProp.GetString(), out var sdVal) ? sdVal : DateTime.MinValue; // gets submission date
            DateTime claimPeriod = claimElement.TryGetProperty("ClaimPeriod", out var cpProp) && DateTime.TryParse(cpProp.GetString(), out var cpVal) ? cpVal : DateTime.MinValue; // gets claim period
            decimal totalAmount = claimElement.TryGetProperty("TotalAmount", out var taProp) ? taProp.GetDecimal() : 0m; // gets total amount

            string statusString = claimElement.TryGetProperty("Status", out var stProp) ? (stProp.GetString() ?? "") : ""; // gets status string
            var status = Enum.TryParse<ClaimStatusEnum>(statusString, out var parsed) ? parsed : ClaimStatusEnum.Submitted; // parses status

            string originalFileName = claimElement.TryGetProperty("OriginalFileName", out var ofnProp) ? (ofnProp.GetString() ?? "") : ""; // gets original file name
            string encryptedFileName = claimElement.TryGetProperty("EncryptedFileName", out var efnProp) ? (efnProp.GetString() ?? "") : ""; // gets encrypted file name

            var model = new ClaimDetailsViewModel
            {
                ClaimID = id,
                LecturerName = lecturerName,
                SubmissionDate = submissionDate,
                ClaimPeriod = claimPeriod,
                TotalAmount = totalAmount,
                Status = status,
                Documents = new List<SupportingDocumentViewModel>()
            };

            if (!string.IsNullOrEmpty(encryptedFileName)) // add supporting document if exists
            {
                model.Documents.Add(new SupportingDocumentViewModel
                {
                    FileName = string.IsNullOrEmpty(originalFileName) ? encryptedFileName : originalFileName,
                    FileUrl = Url.Action("Download", "File", new { id = model.ClaimID }) ?? string.Empty, // generates the download url
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
