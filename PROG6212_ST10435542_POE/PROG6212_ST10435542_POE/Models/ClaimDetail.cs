namespace PROG6212_ST10435542_POE.Models
{
    public class ClaimDetail
    {
        public int ClaimDetailID { get; set; }
        public int ClaimID { get; set; }
        public DateTime DateWorked { get; set; }
        public int HoursWorked { get; set; }
        public string TaskDescription { get; set; }

        public MonthlyClaim MonthlyClaim { get; set; }
    }
}
