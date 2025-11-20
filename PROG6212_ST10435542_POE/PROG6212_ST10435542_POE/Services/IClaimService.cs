using Microsoft.AspNetCore.Identity;
using PROG6212_ST10435542_POE.Models;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;

namespace PROG6212_ST10435542_POE.Services
{
    public interface IClaimService
    {
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);

        Task<List<MonthlyClaim>> GenerateInvoiceReportDataAsync(DateTime month);

        Task<int> SubmitNewClaimAsync(MonthlyClaim claim);
        Task<bool> IsHoursValidForMonth(string lecturerId, DateTime month, decimal hoursWorked);
        Task<List<MonthlyClaim>> GetLecturerClaimsAsync(string lecturerId);

        Task<MonthlyClaim> GetClaimByIdAsync(int claimId);
        Task<List<MonthlyClaim>> GetPendingClaimsAsync(UserRoleEnum nextApproverRole);
        Task<bool> ApproveClaimAsync(int claimId, UserRoleEnum approverRole, string notes);
        Task<bool> RejectClaimAsync(int claimId, UserRoleEnum approverRole, string notes);
    }
}
