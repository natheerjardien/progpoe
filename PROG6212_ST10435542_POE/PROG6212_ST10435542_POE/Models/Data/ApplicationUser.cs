using Microsoft.AspNetCore.Identity;
using PROG6212_ST10435542_POE.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_ST10435542_POE.Models.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRoleEnum UserRole { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; } // all the lecturer financials will be stored here
    }
}
