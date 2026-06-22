using AutoMapper;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.prescription;
using depi__project.viewmodels.prescriptionitems;
using Microsoft.AspNetCore.Mvc;

namespace depi__project.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMapper _mapper;

        public PrescriptionController(IPrescriptionRepository prescriptionRepository, IMapper mapper)
        {
            _prescriptionRepository = prescriptionRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response = await _prescriptionRepository.GetAllAsync(pagination, searchTerm);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve prescriptions.";
                return View(new List<ResponsePrescriptionVM>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _prescriptionRepository.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(response.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? visitId)
        {
            return await BuildCreateView(visitId);
        }

        [HttpGet]
        public Task<IActionResult> AddPrescription(int visitId) => BuildCreateView(visitId);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddPrescriptionVM model)
        {
            model.items ??= new List<AddPrescriptionItemVM>();
            model.items = model.items.Where(i => i.mdeicineid > 0).ToList();

            if (!model.items.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one prescription item is required.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateCreateListsAsync(model);
                return View(model);
            }

            var response = await _prescriptionRepository.AddAsync(model);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                await PopulateCreateListsAsync(model);
                TempData["ErrorMessage"] = response.Message ?? "Failed to create prescription.";
                return View(model);
            }

            TempData["SuccessMessage"] = response.Message ?? "Prescription created successfully.";
            return RedirectToAction(nameof(Details), new { id = response.Data!.prescriptionid });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _prescriptionRepository.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new UpdatePrescriptionvm
            {
                prescriptionid = response.Data!.prescriptionid,
                prescriptiondate = response.Data.prescriptiondate,
                notes = response.Data.notes,
                visitid = response.Data.visitid,
                PatientName = response.Data.patientname,
                items = response.Data.prescriptionitems.Select(i => new UpdatePrescriptionitemvm
                {
                    prescriptionitemid = i.prescriptionitemid,
                    mdeicineid = i.mdeicineid,
                    quantity = i.quantity,
                    Dosage = i.Dosage,
                    Frequency = i.Frequency,
                    Duration = i.Duration,
                    Instructions = i.Instructions,
                    prescriptionid = i.prescriptionid
                }).ToList()
            };

            await PopulateEditListsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePrescriptionvm model)
        {
            model.items ??= new List<UpdatePrescriptionitemvm>();
            model.items = model.items.Where(i => i.mdeicineid > 0).ToList();

            if (!model.items.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one prescription item is required.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateEditListsAsync(model);
                return View(model);
            }

            var response = await _prescriptionRepository.UpdateAsync(model);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                await PopulateEditListsAsync(model);
                TempData["ErrorMessage"] = response.Message ?? "Failed to update prescription.";
                return View(model);
            }

            TempData["SuccessMessage"] = response.Message ?? "Prescription updated successfully.";
            return RedirectToAction(nameof(Details), new { id = response.Data!.prescriptionid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _prescriptionRepository.DeleteAsync(id);
            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Message ?? (response.Success ? "Prescription deleted successfully." : "Failed to delete prescription.");

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> BuildCreateView(int? visitId)
        {
            var medicines = await _prescriptionRepository.GetMedicinesForSelectAsync();
            var vm = new AddPrescriptionVM
            {
                visitid = visitId ?? 0,
                prescriptiondate = DateTime.Today,
                Visits = await _prescriptionRepository.GetVisitsForSelectAsync(visitId),
                items = new List<AddPrescriptionItemVM>
                {
                    new AddPrescriptionItemVM { medicines = medicines }
                }
            };

            if (visitId.HasValue && visitId.Value > 0)
            {
                vm.PatientName = vm.Visits.FirstOrDefault(v => v.Selected)?.Text;
            }

            return View("Create", vm);
        }

        private async Task PopulateCreateListsAsync(AddPrescriptionVM model)
        {
            var medicines = await _prescriptionRepository.GetMedicinesForSelectAsync();
            model.Visits = await _prescriptionRepository.GetVisitsForSelectAsync(model.visitid > 0 ? model.visitid : null);

            foreach (var item in model.items)
            {
                item.medicines = medicines;
            }
        }

        private async Task PopulateEditListsAsync(UpdatePrescriptionvm model)
        {
            var medicines = await _prescriptionRepository.GetMedicinesForSelectAsync();

            foreach (var item in model.items)
            {
                item.medicines = medicines;
            }
        }
    }
}
