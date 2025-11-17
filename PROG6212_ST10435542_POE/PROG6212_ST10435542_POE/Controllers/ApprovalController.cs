using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.Approval;

namespace PROG6212_ST10435542_POE.Controllers
{
// As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing approver-related actions such as adding approval settings, view pending claims, view processe claims, and review claims made by lecturer
// Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    public class ApprovalController : Controller
    {
        // displays the form to configure approval rules
        public IActionResult ApprovalWorkflowSettings()
        {
            return View(new ApprovalWorkflowSettingsViewModel());
        }

        // displays claims awaiting review by Coordinators/Managers
        public IActionResult PendingClaims()
        {
            return View(new List<PendingClaimViewModel>());
        }

        // displays claims that have already been reviewed
        public IActionResult ProcessedClaims()
        {
            return View(new List<ProcessedClaimViewModel>());
        }

        // displays detailed information for a specific claim
        public IActionResult ReviewClaim(int id)
        {
            return View(new ClaimDetailsViewModel());
        }
    }
}

/* References:

IIEVC School of Computer Science, 2025. CLDV6212 Building a Modern Web App with Azure Table Storage & ASP.NET Core MVC - Part 1. [video online] 
Available at: <https://youtu.be/Txp7VYUMBGQ?si=5sD7TV0vS90-pPHY>
[Accessed 14 September 2025].

*/
