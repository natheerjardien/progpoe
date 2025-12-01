using Microsoft.AspNetCore.Identity;
using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_ST10435542_POE.Models.Data
{
// Accoring to Microsoft Learn (n.d.), the ApplicationUser class extends the IdentityUser class to include additional properties for user profiles
// Instead of using the default IdentityUser, I created this ApplicationUser class to add custom properties like FirstName, LastName, UserRole, and HourlyRate
    public class ApplicationUser : IdentityUser // extends IdentityUser to include additional properties
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRoleEnum UserRole { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; } // all the lecturer financials will be stored here
    }
}

/* References:

Microsoft Learn, (n.d.). Customizing Identity Models in ASP.NET Core. [online] 
Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-10.0>
[Accessed 17 November 2025].

*/
