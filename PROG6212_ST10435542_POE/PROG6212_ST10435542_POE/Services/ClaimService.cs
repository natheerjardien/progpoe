using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6212_ST10435542_POE.Models;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;

namespace PROG6212_ST10435542_POE.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClaimService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- HR User Management Methods ---

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            // Returns all users including their HR-added properties
            return await _userManager.Users.OrderBy(u => u.LastName).ToListAsync();
        }

        public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null) return null;

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.NormalizedEmail = user.Email.ToUpper();
            existingUser.UserName = user.Email;
            existingUser.NormalizedUserName = user.Email.ToUpper();
            existingUser.HourlyRate = user.HourlyRate;
            existingUser.UserRole = user.UserRole;

            var currentRoles = await _userManager.GetRolesAsync(existingUser);
            var newRole = user.UserRole.ToString();

            if (currentRoles.FirstOrDefault() != newRole)
            {
                await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                await _userManager.AddToRoleAsync(existingUser, newRole);
            }

            var result = await _userManager.UpdateAsync(existingUser);
            return result.Succeeded ? existingUser : null;
        }

        // --- HR Reporting Method (LINQ based) ---

        public async Task<List<MonthlyClaim>> GenerateInvoiceReportDataAsync(DateTime month)
        {
            // LINQ query to get all APPROVED claims for the specified month
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1);

            return await _context.MonthlyClaims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatusEnum.ApprovedByCoordinator &&
                            c.ClaimPeriod >= startDate && c.ClaimPeriod < endDate)
                .OrderBy(c => c.Lecturer.LastName)
                .ToListAsync();
        }

        // --- Lecturer Claim Method Skeeltons (finisih later) ---

        public Task<int> SubmitNewClaimAsync(MonthlyClaim claim)
        {
            return Task.FromResult(-1);
        }

        public Task<bool> IsHoursValidForMonth(string lecturerId, DateTime month, decimal hoursWorked)
        {
            return Task.FromResult(true);
        }

        public Task<List<MonthlyClaim>> GetLecturerClaimsAsync(string lecturerId)
        {
            return Task.FromResult(new List<MonthlyClaim>());
        }

        // --- Coordinator & Manager Approval Skeletons (finish later) ---

        public Task<MonthlyClaim> GetClaimByIdAsync(int claimId)
        {
            return Task.FromResult<MonthlyClaim>(null);
        }

        public Task<List<MonthlyClaim>> GetPendingClaimsAsync(UserRoleEnum nextApproverRole)
        {
            // Implementation detail deferred, but stubbed for compilation
            return Task.FromResult(new List<MonthlyClaim>());
        }

        public Task<bool> ApproveClaimAsync(int claimId, UserRoleEnum approverRole, string notes)
        {
            return Task.FromResult(false);
        }

        public Task<bool> RejectClaimAsync(int claimId, UserRoleEnum approverRole, string notes)
        {
            return Task.FromResult(false);
        }
    }
}
