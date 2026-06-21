using AutoMapper;
using depi__project.enums;
using depi__project.services.interfaces;
using depi__project.viewmodels.Appoinment;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace depi__project.Controllers
{
    public class AppoinmentController : Controller
    {
        private readonly IAppoinment appoinmentService;
        private readonly IMapper mapper;

        public AppoinmentController(IAppoinment appoinmentService, IMapper mapper)
        {
            this.appoinmentService = appoinmentService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> ListAppoinments(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? phone = null, DateTime? date = null, AppointmentStatus? status = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response = await appoinmentService.GetAllAsync(pagination, searchTerm, phone, date, status);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Phone = phone;
            ViewBag.Date = date?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve appointments.";
                return View(new List<ResponseAppoimentVM>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> AppoinmentDetails(int id)
        {
            var response = await appoinmentService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Appointment not found.";
                return RedirectToAction(nameof(ListAppoinments));
            }

            return View(response.Data);
        }

        public async Task<IActionResult> AddAppoinment()
        {
            var vm = new AddAppoinmentVM
            {
                Appoinmentdate = DateTime.Today,
                startat = DateTime.Now,
                endat = DateTime.Now.AddMinutes(30),
                doctorsvm = (await appoinmentService.GetDoctorsSelectListAsync()).ToList(),
                receptionists = (await appoinmentService.GetReceptionistsSelectListAsync()).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddAppoinment(AddAppoinmentVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.doctorsvm = (await appoinmentService.GetDoctorsSelectListAsync()).ToList();
                vm.receptionists = (await appoinmentService.GetReceptionistsSelectListAsync()).ToList();
                return View("AddAppoinment", vm);
            }

            var response = await appoinmentService.AddAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                vm.doctorsvm = (await appoinmentService.GetDoctorsSelectListAsync()).ToList();
                vm.receptionists = (await appoinmentService.GetReceptionistsSelectListAsync()).ToList();
                TempData["ErrorMessage"] = response.Message ?? "Failed to book appointment.";
                return View("AddAppoinment", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Appointment booked successfully.";
            return RedirectToAction(nameof(AppoinmentDetails), new { id = response.Data.appoimentid });
        }

        public async Task<IActionResult> UpdateAppoinment(int id)
        {
            var response = await appoinmentService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Appointment not found.";
                return RedirectToAction(nameof(ListAppoinments));
            }

            var vm = new UpdateAppoinment
            {
                AppoinmentId = response.Data.appoimentid,
                Appoinmentdate = response.Data.Appoinmentdate,
                startat = response.Data.startat,
                endat = response.Data.endat,
                cost = response.Data.cost,
                type = response.Data.type,
                resptionistidid = response.Data.resptionistidid,
                notes = response.Data.notes,
                doctorid = response.Data.doctorid,
                PhoneNumber = response.Data.PhoneNumber
            };

            ViewBag.Doctors = await appoinmentService.GetDoctorsSelectListAsync();
            ViewBag.Receptionists = await appoinmentService.GetReceptionistsSelectListAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdateAppoinment(UpdateAppoinment vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await appoinmentService.GetDoctorsSelectListAsync();
                ViewBag.Receptionists = await appoinmentService.GetReceptionistsSelectListAsync();
                return View("UpdateAppoinment", vm);
            }

            var response = await appoinmentService.UpdateAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                ViewBag.Doctors = await appoinmentService.GetDoctorsSelectListAsync();
                ViewBag.Receptionists = await appoinmentService.GetReceptionistsSelectListAsync();
                TempData["ErrorMessage"] = response.Message ?? "Failed to update appointment.";
                return View("UpdateAppoinment", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Appointment updated successfully.";
            return RedirectToAction(nameof(AppoinmentDetails), new { id = response.Data.appoimentid });
        }

        public async Task<IActionResult> UpdateAppoinmentState(int id)
        {
            var response = await appoinmentService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Appointment not found.";
                return RedirectToAction(nameof(ListAppoinments));
            }

            var vm = new UpdateAppoinmentStateVM
            {
                AppoinmentId = response.Data.appoimentid,
                Status = response.Data.status,
                patientname = response.Data.Patient?.patientname ?? "N/A",
                phone = response.Data.PhoneNumber
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdateAppoinmentState(UpdateAppoinmentStateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateAppoinmentState", vm);
            }

            var response = await appoinmentService.UpdateStatusAsync(vm);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to update appointment status.";
                return View("UpdateAppoinmentState", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Appointment status updated successfully.";
            return RedirectToAction(nameof(AppoinmentDetails), new { id = vm.AppoinmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAppoinment(int id)
        {
            var response = await appoinmentService.DeleteAsync(id);
            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] = response.Message;
            return RedirectToAction(nameof(ListAppoinments));
        }
    }
}
