namespace PROG6212_ST10435542_POE.Models
{
    public class Lecturer
    {
        public int UserID { get; set; }
        
        public decimal HourlyRate { get; set; } // lecturer specific financial details
        public string BankDetails { get; set; }

        public User User { get; set; } // navigation property to the User
        public ICollection<MonthlyClaim>? MonthlyClaims { get; set; } // lecturer cans ubmit multple claims
    }
}
