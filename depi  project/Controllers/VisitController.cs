using AutoMapper;
using depi__project.enums;
using depi__project.services.interfaces;
using depi__project.viewmodels.Visit;
using depi__project.viewmodels.VisitRepo;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace depi__project.Controllers
{
    public class VisitController : Controller
    {
        private readonly IVisit visitService;
        private readonly IMapper mapper;

        public VisitController(IVisit visitService, IMapper mapper)
        {
            this.visitService = visitService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> ListVisits(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, VisitStatus? status = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response = await visitService.GetAllAsync(pagination, searchTerm, status);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve visits.";
                return View(new List<ResponseVisitVM>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> VisitDetails(int id)
        {
            var response = await visitService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Visit not found.";
                return RedirectToAction(nameof(ListVisits));
            }

            return View(response.Data);
        }

        public async Task<IActionResult> AddVisit(int? appointmentId)
        {
            var appointments = await visitService.GetPendingAppointmentsForVisitAsync();
            ViewBag.Appointments = appointments;

            var vm = new AddVisit();
            if (appointmentId.HasValue)
            {
                vm.appoinmentid = appointmentId.Value;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddVisit(AddVisit vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Appointments = await visitService.GetPendingAppointmentsForVisitAsync();
                return View("AddVisit", vm);
            }

            var response = await visitService.AddAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                ViewBag.Appointments = await visitService.GetPendingAppointmentsForVisitAsync();
                TempData["ErrorMessage"] = response.Message ?? "Failed to start visit.";
                return View("AddVisit", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Visit started successfully.";
            return RedirectToAction(nameof(VisitDetails), new { id = response.Data.visitid });
        }

        public async Task<IActionResult> UpdateVisit(int id)
        {
            var response = await visitService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Visit not found.";
                return RedirectToAction(nameof(ListVisits));
            }

            var vm = new UpdateVisitVM
            {
                visitid = response.Data.visitid,
                notes = response.Data.notes,
                diagnosis = response.Data.diagnosis,
                visitstatus = response.Data.visitstatus
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdateVisit(UpdateVisitVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateVisit", vm);
            }

            var response = await visitService.UpdateAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                TempData["ErrorMessage"] = response.Message ?? "Failed to update visit.";
                return View("UpdateVisit", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Visit updated successfully.";
            return RedirectToAction(nameof(VisitDetails), new { id = response.Data.visitid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveVisit(int id)
        {
            var response = await visitService.DeleteAsync(id);
            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] = response.Message;
            return RedirectToAction(nameof(ListVisits));
        }
    }
}
