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

        Task<bool> ResetUserPasswordAsync(string userId, string newPassword);
    }
}
