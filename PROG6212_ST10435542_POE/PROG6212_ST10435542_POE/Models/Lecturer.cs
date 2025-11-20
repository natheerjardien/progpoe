using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_ST10435542_POE.Models
{
    public class Lecturer
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; } // lecturer specific financial details
        public string BankDetails { get; set; }

        public virtual User User { get; set; } // navigation property to the User
        public ICollection<MonthlyClaim>? MonthlyClaims { get; set; } // lecturer cans ubmit multple claims
    }
}
