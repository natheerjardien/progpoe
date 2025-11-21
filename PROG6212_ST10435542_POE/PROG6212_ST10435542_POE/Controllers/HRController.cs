using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Models.ViewModels.HR;
using PROG6212_ST10435542_POE.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization; // to show the currency as rands in the report

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

                if (!string.IsNullOrWhiteSpace(model.Password)) // checks if the password field was filled
                {
                    await _claimService.ResetUserPasswordAsync(user.Id, model.Password);
                }

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

                if (!claims.Any())
                {
                    ModelState.AddModelError(string.Empty, "No approved claims found for the selected month.");
                    return View(model);
                }

                // this section of code generates the pdf using QuestPDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text($"Approved Claims Report - {model.ReportMonth:MMMM yyyy}")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Lecturer");
                                    header.Cell().Element(CellStyle).Text("Email");
                                    header.Cell().Element(CellStyle).Text("Hours");
                                    header.Cell().Element(CellStyle).Text("Rate");
                                    header.Cell().Element(CellStyle).Text("Total");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                    }
                                });

                                var zarCurrency = CultureInfo.GetCultureInfo("en-ZA");

                                foreach (var item in claims)
                                {
                                    table.Cell().Element(CellStyle).Text($"{item.Lecturer.FirstName} {item.Lecturer.LastName}");
                                    table.Cell().Element(CellStyle).Text(item.Lecturer.Email);
                                    table.Cell().Element(CellStyle).Text(item.TotalHoursWorked.ToString("F2"));
                                    table.Cell().Element(CellStyle).Text(item.Lecturer.HourlyRate.ToString("C", zarCurrency));
                                    table.Cell().Element(CellStyle).Text(item.TotalAmount.ToString("C", zarCurrency));

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                    }
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                var fileName = $"InvoiceReport_{model.ReportMonth:yyyyMM}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }

            return View(model);
        }
    }
}
