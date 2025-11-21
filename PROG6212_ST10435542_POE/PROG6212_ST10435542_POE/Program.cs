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

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(UserRoleEnum.HR.ToString(), policy => policy.RequireRole(UserRoleEnum.HR.ToString()));
                options.AddPolicy(UserRoleEnum.Lecturer.ToString(), policy => policy.RequireRole(UserRoleEnum.Lecturer.ToString()));
                options.AddPolicy(UserRoleEnum.ProgrammeCoordinator.ToString(), policy => policy.RequireRole(UserRoleEnum.ProgrammeCoordinator.ToString()));
                options.AddPolicy(UserRoleEnum.AcademicManager.ToString(), policy => policy.RequireRole(UserRoleEnum.AcademicManager.ToString()));
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IClaimService, ClaimService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            QuestPDF.Settings.License = LicenseType.Community; // to print the invoice report as a pdf

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.Use(async (context, next) =>
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
