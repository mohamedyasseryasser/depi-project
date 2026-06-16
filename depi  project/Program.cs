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
        public static void Main(string[] args)
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
            builder.Services.AddControllersWithViews();
            //auto mapping 
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Auth/Login";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                context.Database.CanConnect(); // يفتح connection بدري
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
