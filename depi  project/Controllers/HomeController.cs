using System.Diagnostics;
using depi__project.enums;
using depi__project.Models;
using depi__project.viewmodels.Appoinment;
using depi__project.viewmodels.home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace depi__project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Context context;
        private readonly UserManager<Aplicationuser> userManager;
        private readonly ILogger<HomeController> logger;

        public HomeController(Context context, UserManager<Aplicationuser> userManager, ILogger<HomeController> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            var roles = currentUser == null ? new List<string>() : (await userManager.GetRolesAsync(currentUser)).ToList();
            var primaryRole = roles.FirstOrDefault() ?? "Staff";

            var vm = new dashboardvm
            {
                RoleName = primaryRole,
                DisplayName = currentUser?.UserName ?? "User"
            };

            if (User.IsInRole("Admin"))
            {
                var appointmentScope = context.Appointments.AsNoTracking();
                vm.patientcount = await context.Patients.CountAsync(p => p.isvalid);
                vm.appoinmentcount = await appointmentScope.CountAsync();
                vm.totaldoctors = await context.Doctors.CountAsync(d => d.status != userstatus.inactive);
                vm.departmentcount = await context.Departments.CountAsync(d => d.isactive);
                vm.medicinecount = await context.Medicines.CountAsync(m => !m.IsDeleted);
                vm.lowstockcount = await context.Medicines.CountAsync(m => !m.IsDeleted && m.StockQuantity <= m.RecordLevel);
                vm.pendinginvoicecount = await context.Invoices.CountAsync(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled);
                vm.revenue = await context.Invoices
                    .Where(i => i.Status == InvoiceStatus.Paid)
                    .SumAsync(i => (decimal?)i.FinalAmount) ?? 0m;
                vm.items = await GetAppointmentsQuery()
                    .OrderByDescending(a => a.Appoinmentdate)
                    .ThenByDescending(a => a.startat)
                    .Take(5)
                    .Select(a => MapAppointment(a))
                    .ToListAsync();
                await PopulateAppointmentStatsAsync(vm, appointmentScope);
                await PopulateInvoiceStatsAsync(vm);
            }
            else if (User.IsInRole("Doctor") && currentUser != null)
            {
                var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.userid == currentUser.Id);
                if (doctor != null)
                {
                    var appointmentScope = context.Appointments.AsNoTracking().Where(a => a.doctorid == doctor.DoctorId);
                    vm.appoinmentcount = await appointmentScope.CountAsync(a => a.status != AppointmentStatus.Cancelled);
                    vm.patientcount = await context.Appointments
                        .Where(a => a.doctorid == doctor.DoctorId)
                        .Select(a => a.patientid)
                        .Distinct()
                        .CountAsync();
                    vm.visitcount = await context.Visits.CountAsync(v => v.Appoinment != null && v.Appoinment.doctorid == doctor.DoctorId);
                    vm.items = await GetAppointmentsQuery()
                        .Where(a => a.doctorid == doctor.DoctorId)
                        .OrderBy(a => a.Appoinmentdate)
                        .ThenBy(a => a.startat)
                        .Take(5)
                        .Select(a => MapAppointment(a))
                        .ToListAsync();
                    await PopulateAppointmentStatsAsync(vm, appointmentScope);
                }
            }
            else if (User.IsInRole("Receptionist") && currentUser != null)
            {
                var receptionist = await context.resptionists.FirstOrDefaultAsync(r => r.userid == currentUser.Id);
                if (receptionist != null)
                {
                    var appointmentScope = context.Appointments.AsNoTracking().Where(a => a.resptionistidid == receptionist.resptionistid);
                    vm.patientcount = await context.Patients.CountAsync(p => p.isvalid);
                    vm.appoinmentcount = await appointmentScope.CountAsync();
                    vm.visitcount = await context.Visits.CountAsync();
                    vm.pendinginvoicecount = await context.Invoices.CountAsync(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled);
                    vm.items = await GetAppointmentsQuery()
                        .Where(a => a.resptionistidid == receptionist.resptionistid)
                        .OrderBy(a => a.Appoinmentdate)
                        .ThenBy(a => a.startat)
                        .Take(5)
                        .Select(a => MapAppointment(a))
                        .ToListAsync();
                    await PopulateAppointmentStatsAsync(vm, appointmentScope);
                    await PopulateInvoiceStatsAsync(vm);
                }
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IQueryable<Appoinment> GetAppointmentsQuery()
        {
            return context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.user);
        }

        private async Task PopulateAppointmentStatsAsync(dashboardvm vm, IQueryable<Appoinment> query)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var trendStart = today.AddDays(-6);

            vm.todayappointmentcount = await query.CountAsync(a => a.Appoinmentdate >= today && a.Appoinmentdate < tomorrow);
            vm.pendingAppointmentCount = await query.CountAsync(a => a.status == AppointmentStatus.Pending);
            vm.confirmedAppointmentCount = await query.CountAsync(a => a.status == AppointmentStatus.Confirmed);
            vm.completedAppointmentCount = await query.CountAsync(a => a.status == AppointmentStatus.Completed);
            vm.cancelledAppointmentCount = await query.CountAsync(a => a.status == AppointmentStatus.Cancelled);
            vm.noShowAppointmentCount = await query.CountAsync(a => a.status == AppointmentStatus.NoShow);

            var trendDates = await query
                .Where(a => a.Appoinmentdate >= trendStart && a.Appoinmentdate < tomorrow)
                .Select(a => a.Appoinmentdate)
                .ToListAsync();
            var countsByDay = trendDates
                .GroupBy(date => date.Date)
                .ToDictionary(group => group.Key, group => group.Count());

            for (var day = trendStart; day <= today; day = day.AddDays(1))
            {
                vm.appointmentTrendLabels.Add(day.ToString("ddd"));
                vm.appointmentTrendCounts.Add(countsByDay.GetValueOrDefault(day));
            }
        }

        private async Task PopulateInvoiceStatsAsync(dashboardvm vm)
        {
            vm.paidInvoiceCount = await context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Paid);
            vm.pendinginvoicecount = await context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Pending);
            vm.cancelledInvoiceCount = await context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Cancelled);
        }

        private static ResponseAppoimentVM MapAppointment(Appoinment appointment)
        {
            return new ResponseAppoimentVM
            {
                appoimentid = appointment.appoimentid,
                PhoneNumber = appointment.PhoneNumber,
                Appoinmentdate = appointment.Appoinmentdate,
                startat = appointment.startat,
                endat = appointment.endat,
                notes = appointment.notes,
                cost = appointment.cost,
                type = appointment.type,
                status = appointment.status,
                Patient = appointment.Patient == null ? null : new depi__project.viewmodels.Patient.ResponsePatientVM
                {
                    patientid = appointment.Patient.patientid,
                    patientname = appointment.Patient.patientname,
                    phonenumber = appointment.Patient.phonenumber,
                    Gender = appointment.Patient.Gender,
                    datebirth = appointment.Patient.datebirth,
                    nationalid = appointment.Patient.nationalid,
                    isvalid = appointment.Patient.isvalid
                },
                DoctorName = appointment.Doctor?.user?.UserName
            };
        }
    }
}
