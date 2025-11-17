using Microsoft.AspNetCore.Mvc;
using PROG6212_ST10435542_POE.Models.ViewModels.Account;

namespace PROG6212_ST10435542_POE.Controllers
{
// As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing account-related actions such as adding user through logins and registering new users
// Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

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
