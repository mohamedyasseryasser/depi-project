//using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.services.reporesity;

namespace smart_clinic
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 🔗 Connection String (SQL Server)
            builder.Services.AddDbContext<Context>(options =>
           options.UseSqlServer(
               builder.Configuration.GetConnectionString("DefaultConnection")
           ).LogTo(Console.WriteLine, LogLevel.Information)
       );
            // 🔐 Identity Setup
            builder.Services.AddIdentity<Aplicationuser, IdentityRole>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();
            // Add services to the container.
            builder.Services.AddScoped<IUser, User>();
            builder.Services.AddScoped<IAuth, Auth>();
            builder.Services.AddScoped<IDepartment,DepartmentRepo>();
            builder.Services.AddScoped<IPatient, PatientRepo>();
            builder.Services.AddScoped<IAppoinment, AppoinmentRepo>();
            builder.Services.AddScoped<IVisit, VisitRepo>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            builder.Services.AddControllersWithViews();
            //auto mapping 
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Auth/Login";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<Context>();
                var userManager = services.GetRequiredService<UserManager<Aplicationuser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                context.Database.Migrate();

                foreach (var role in new[] { "Admin", "Doctor", "Receptionist" })
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                if (!await userManager.Users.AnyAsync())
                {
                    var adminUser = new Aplicationuser
                    {
                        UserName = "admin",
                        Email = "admin@clinic.local",
                        PhoneNumber = "01012345678",
                        address = "Cairo Clinic",
                        Gender = depi__project.enums.Gender.Male,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        context.Admins.Add(new Admin
                        {
                            userid = adminUser.Id,
                            permissions = "full",
                            status = depi__project.enums.userstatus.active
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
