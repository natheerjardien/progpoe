using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG6212_ST10435542_POE.Models;
using PROG6212_ST10435542_POE.Models.Data;
using PROG6212_ST10435542_POE.Models.Enums;
using PROG6212_ST10435542_POE.Services;
using QuestPDF.Infrastructure;

namespace PROG6212_ST10435542_POE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

// According to Microsoft Learn (n.d.), the following code configures services for the ASP.NET Core application, including database context, identity, authorization policies, and session settings
// I configured the connection string for the database context using SQL Server and set up identity with custom password requirements
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => // configuring password requirements
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddAuthorization(options => // defining role-based authorization policies
            {
                options.AddPolicy(UserRoleEnum.HR.ToString(), policy => policy.RequireRole(UserRoleEnum.HR.ToString()));
                options.AddPolicy(UserRoleEnum.Lecturer.ToString(), policy => policy.RequireRole(UserRoleEnum.Lecturer.ToString()));
                options.AddPolicy(UserRoleEnum.ProgrammeCoordinator.ToString(), policy => policy.RequireRole(UserRoleEnum.ProgrammeCoordinator.ToString()));
                options.AddPolicy(UserRoleEnum.AcademicManager.ToString(), policy => policy.RequireRole(UserRoleEnum.AcademicManager.ToString()));
            });

// According to Microsoft Learn (n.d.), session management is configured to maintain user state across requests
// I added this builder service to enable session management in the application and accessed the ssession in various controllers to store user-specific data
            builder.Services.AddSession(options => // configuring session settings
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IClaimService, ClaimService>(); // registering application services for dependency injection
            builder.Services.AddScoped<IFileStorageService, FileStorageService>(); // service for handling file storage operations

            QuestPDF.Settings.License = LicenseType.Community; // to print the invoice report as a pdf

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.Use(async (context, next) => // middleware to handle 401 and 403 status codes
            {
                await next();
                if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
                {
                    context.Response.Redirect("/Account/Login");
                }
                else if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
                {
                    context.Response.Redirect("/Account/AccessDenied");
                }
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

/* References:

Microsoft Learn, (n.d.). ASP.NET Core MVC with Entity Framework Core - Tutorial. [online] 
Available at: <https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-8.0>
[Accessed 14 September 2025].

Microsoft Learn, (n.d.). Session and state management in ASP.NET Core. [online] 
Available at: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-10.0>
[Accessed 16 November 2025].

*/
