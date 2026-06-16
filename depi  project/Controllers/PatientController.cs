using AutoMapper;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.Patient;
using Microsoft.AspNetCore.Mvc;

namespace depi__project.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatient patientService;
        private readonly IMapper mapper;

        public PatientController(IPatient patientService, IMapper mapper)
        {
            this.patientService = patientService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> ListPatients(int pageNumber = 1, int pageSize = 10, string searchTerm = null, bool? isvalid = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response = await patientService.GetAllAsync(pagination, searchTerm, isvalid);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.IsValid = isvalid;

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve patients.";
                return View(new List<ResponsePatientVM>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> PatientDetails(int id)
        {
            var response = await patientService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Patient not found.";
                return RedirectToAction(nameof(ListPatients));
            }

            return View(response.Data);
        }

        public IActionResult AddPatient()
        {
            return View(new AddPatientVM { datebirth = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddPatient(AddPatientVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("AddPatient", vm);
            }

            var response = await patientService.AddAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                TempData["ErrorMessage"] = response.Message ?? "Failed to add patient.";
                return View("AddPatient", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Patient added successfully.";
            return RedirectToAction(nameof(PatientDetails), new { id = response.Data.patientid });
        }

        public async Task<IActionResult> UpdatePatient(int id)
        {
            var response = await patientService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Patient not found.";
                return RedirectToAction(nameof(ListPatients));
            }

            var vm = mapper.Map<UpdatePatientVM>(response.Data);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdatePatient(UpdatePatientVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdatePatient", vm);
            }

            var response = await patientService.UpdateAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                TempData["ErrorMessage"] = response.Message ?? "Failed to update patient.";
                return View("UpdatePatient", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Patient updated successfully.";
            return RedirectToAction(nameof(PatientDetails), new { id = response.Data.patientid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePatient(int id)
        {
            var response = await patientService.DeleteAsync(id);
            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Message ?? (response.Success ? "Patient deactivated successfully." : "Failed to deactivate patient.");

            return RedirectToAction(nameof(ListPatients));
        }
    }
}
