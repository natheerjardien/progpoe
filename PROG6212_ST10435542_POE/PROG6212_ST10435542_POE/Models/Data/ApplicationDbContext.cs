using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using PROG6212_ST10435542_POE.Models.Enums;

namespace PROG6212_ST10435542_POE.Models.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<MonthlyClaim> MonthlyClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("Identity");
            builder.Entity<ApplicationUser>(entity => { entity.ToTable("Users"); });
            builder.Entity<IdentityRole>(entity => { entity.ToTable("Roles"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "hr", Name = UserRoleEnum.HR.ToString(), NormalizedName = UserRoleEnum.HR.ToString().ToUpper() },
                new IdentityRole { Id = "lecturer", Name = UserRoleEnum.Lecturer.ToString(), NormalizedName = UserRoleEnum.Lecturer.ToString().ToUpper() },
                new IdentityRole { Id = "coordinator", Name = UserRoleEnum.ProgrammeCoordinator.ToString(), NormalizedName = UserRoleEnum.ProgrammeCoordinator.ToString().ToUpper() },
                new IdentityRole { Id = "manager", Name = UserRoleEnum.AcademicManager.ToString(), NormalizedName = UserRoleEnum.AcademicManager.ToString().ToUpper() }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            var hasher = new PasswordHasher<ApplicationUser>();
            var hrUser = new ApplicationUser
            {
                Id = "hr-user-id-1",
                UserName = "natheer@example.com",
                NormalizedUserName = "NATHEER@EXAMPLE.COM",
                Email = "natheer@example.com",
                NormalizedEmail = "NATHEER@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "HR123!"),
                SecurityStamp = string.Empty,
                FirstName = "HR",
                LastName = "Supervisor",
                UserRole = UserRoleEnum.HR,
                HourlyRate = 0
            };

            builder.Entity<ApplicationUser>().HasData(hrUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "hr",
                UserId = hrUser.Id
            });

            var lecturerUser = new ApplicationUser
            {
                Id = "lecturer-user-id-1",
                UserName = "nj@testing.com",
                NormalizedUserName = "NJ@TESTING.COM",
                Email = "nj@testing.com",
                NormalizedEmail = "NJ@TESTING.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "testing123"),
                SecurityStamp = string.Empty,
                FirstName = "Natheer",
                LastName = "Jardien",
                UserRole = UserRoleEnum.Lecturer,
                HourlyRate = 250.50m
            };
            builder.Entity<ApplicationUser>().HasData(lecturerUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "lecturer",
                UserId = lecturerUser.Id
            });
        }
    }
}
