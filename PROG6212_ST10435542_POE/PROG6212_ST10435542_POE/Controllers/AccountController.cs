using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.ViewModels.Account;

namespace PROG6212_ST10435542_POE.Controllers
{
// As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing account-related actions such as adding user through logins and registering new users
// Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in successfully.");
                    // Get the logged-in user to determine their role and correct redirect
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    var primaryRole = roles.FirstOrDefault();

                    // Custom redirection logic based on role
                    if (primaryRole == "HR")
                    {
                        return LocalRedirect("/HR/ManageUsers");
                    }
                    else if (primaryRole == "Lecturer")
                    {
                        return LocalRedirect("/Lecturer/SubmitClaim");
                    }
                    // Fallback or use returnUrl
                    return LocalRedirect(returnUrl ?? "/Home/Index");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
                    return View(model);
                }
                if (result.RequiresTwoFactor)
                {
                    // Not implemented, but good to check
                    ModelState.AddModelError(string.Empty, "Two-factor authentication required.");
                    return View(model);
                }

                // Generic failure message to prevent enumeration attacks
                ModelState.AddModelError(string.Empty, "Invalid login attempt: Invalid email or password.");
                return View(model);
            }

            // If Model State is invalid (e.g. required fields missing)
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // --- REGISTER ACTION REMOVED AS PER CHECKLIST: HR creates users ---
        [HttpGet]
        public IActionResult Register()
        {
            // Checklist Item: There should be no register - HR creates the user profiles
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

/* References:

IIEVC School of Computer Science, 2025. CLDV6212 Building a Modern Web App with Azure Table Storage & ASP.NET Core MVC - Part 1. [video online] 
Available at: <https://youtu.be/Txp7VYUMBGQ?si=5sD7TV0vS90-pPHY>
[Accessed 14 September 2025].

*/
