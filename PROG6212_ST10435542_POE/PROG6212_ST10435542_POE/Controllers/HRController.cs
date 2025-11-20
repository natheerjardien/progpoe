using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.HR;
using PROG6212_ST10435542_POE.Services;

namespace PROG6212_ST10435542_POE.Controllers
{
    [Authorize(Roles = "HR")]
    [Route("[controller]/[action]")]
    public class HRController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HRController(IClaimService claimService, UserManager<ApplicationUser> userManager)
        {
            _claimService = claimService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            var viewModel = new UserManagementViewModel
            {
                AvailableRoles = Enum.GetNames(typeof(UserRoleEnum)).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserManagementViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required for new user creation.");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserRole = Enum.Parse<UserRoleEnum>(model.SelectedRole),
                    HourlyRate = model.HourlyRate
                };

                var result = await _claimService.CreateUserAsync(user, model.Password, model.SelectedRole);

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("StatusMessage", $"Successfully created {model.SelectedRole} user: {model.Email}.");
                    return RedirectToAction(nameof(ManageUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            model.AvailableRoles = Enum.GetNames(typeof(UserRoleEnum)).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _claimService.GetAllUsersAsync();

            var viewModel = users.Select(user => new UserManagementViewModel
            {
                UserID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                HourlyRate = user.HourlyRate,
                SelectedRole = user.UserRole.ToString()
            }).ToList();

            // retrives and clears session message
            ViewBag.StatusMessage = HttpContext.Session.GetString("StatusMessage");
            HttpContext.Session.Remove("StatusMessage");

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _claimService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new UserManagementViewModel
            {
                UserID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                HourlyRate = user.HourlyRate,
                SelectedRole = user.UserRole.ToString(),
                AvailableRoles = Enum.GetNames(typeof(UserRoleEnum)).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserManagementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _claimService.GetUserByIdAsync(model.UserID);
                if (user == null) return NotFound();

                // Update fields from model
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.UserRole = Enum.Parse<UserRoleEnum>(model.SelectedRole);
                user.HourlyRate = model.HourlyRate; // updates financial info

                var updatedUser = await _claimService.UpdateUserAsync(user);

                if (updatedUser != null)
                {
                    HttpContext.Session.SetString("StatusMessage", $"User {model.Email} updated successfully.");
                    return RedirectToAction(nameof(ManageUsers));
                }
            }
            model.AvailableRoles = Enum.GetNames(typeof(UserRoleEnum)).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult GenerateReport()
        {
            ViewBag.StatusMessage = HttpContext.Session.GetString("StatusMessage");
            HttpContext.Session.Remove("StatusMessage");
            return View(new GenerateReportViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(GenerateReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var claims = await _claimService.GenerateInvoiceReportDataAsync(model.ReportMonth);

                var reportData = claims.Select(c => new InvoiceReportDetail
                {
                    LecturerName = $"{c.Lecturer.FirstName} {c.Lecturer.LastName}",
                    Email = c.Lecturer.Email,
                    HoursWorked = c.TotalHoursWorked,
                    HourlyRate = c.Lecturer.HourlyRate,
                    TotalAmount = c.TotalAmount,
                    ClaimId = c.ClaimID
                }).ToList();

                if (!reportData.Any())
                {
                    ModelState.AddModelError(string.Empty, "No approved claims found for the selected month.");
                    return View(model);
                }

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Claim ID,Lecturer Name,Email,Hours Worked,Hourly Rate,Total Amount");
                foreach (var item in reportData)
                {
                    csv.AppendLine($"{item.ClaimId},{item.LecturerName},{item.Email},{item.HoursWorked},{item.HourlyRate:C},{item.TotalAmount:C}");
                }

                var fileName = $"InvoiceReport_{model.ReportMonth:yyyyMM}.csv";
                var contentType = "text/csv";

                HttpContext.Session.SetString("StatusMessage", $"Generated invoice report for {model.ReportMonth:MMM yyyy} with {reportData.Count} entries.");

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), contentType, fileName);
            }

            ViewBag.StatusMessage = HttpContext.Session.GetString("StatusMessage");
            HttpContext.Session.Remove("StatusMessage");
            return View(model);
        }
    }
}
