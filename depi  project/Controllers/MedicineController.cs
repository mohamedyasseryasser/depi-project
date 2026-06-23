using System.Security.Claims;
using AutoMapper;
using depi__project.services.interfaces;
using depi__project.viewmodels.medicine;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc;

namespace depi__project.Controllers
{
    public class MedicineController : Controller
    {
        private readonly IMedicineRepository _medicineService;
        private readonly IMapper _mapper;

        public MedicineController(IMedicineRepository medicineService, IMapper mapper)
        {
            _medicineService = medicineService;
            _mapper          = mapper;
        }

        public async Task<IActionResult> ListMedicines(
            int pageNumber = 1, int pageSize = 10,
            string? name = null, int? cat_id = null, bool? isDeleted = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response = await _medicineService.GetAllAsync(pagination, isDeleted, name, cat_id);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.Name = name;
            ViewBag.CatId = cat_id;
            ViewBag.IsDeleted = isDeleted;
            ViewBag.Categories = await _medicineService.GetCategoriesSelectListAsync();

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve medicines.";
                return View(new List<ResponseMedicineVM>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> MedicineDetails(int id)
        {
            var response = await _medicineService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Medicine not found.";
                return RedirectToAction(nameof(ListMedicines));
            }

            return View(response.Data);
        }

        public async Task<IActionResult> AddMedicine()
        {
            var vm = new AddMedicineVM
            {
                Categories = await _medicineService.GetCategoriesSelectListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddMedicine(AddMedicineVM vm)
        {
            vm.user_id ??= User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                vm.Categories = await _medicineService.GetCategoriesSelectListAsync();
                return View("AddMedicine", vm);
            }

            var response = await _medicineService.AddAsync(vm);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to add medicine.";
                vm.Categories = await _medicineService.GetCategoriesSelectListAsync();
                return View("AddMedicine", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Medicine added successfully.";
            return RedirectToAction(nameof(MedicineDetails), new { id = response.Data.medicineId });
        }

        public async Task<IActionResult> UpdateMedicine(int id)
        {
            var response = await _medicineService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Medicine not found.";
                return RedirectToAction(nameof(ListMedicines));
            }

            var vm = _mapper.Map<UpdateMedicineVM>(response.Data);
            vm.Categories = await _medicineService.GetCategoriesSelectListAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdateMedicine(UpdateMedicineVM vm)
        {
            vm.user_id ??= User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                vm.Categories = await _medicineService.GetCategoriesSelectListAsync();
                return View("UpdateMedicine", vm);
            }

            var response = await _medicineService.UpdateAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                    ModelState.AddModelError(string.Empty, error);

                TempData["ErrorMessage"] = response.Message ?? "Failed to update medicine.";
                vm.Categories = await _medicineService.GetCategoriesSelectListAsync();
                return View("UpdateMedicine", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Medicine updated successfully.";
            return RedirectToAction(nameof(MedicineDetails), new { id = response.Data.medicineId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMedicine(int id)
        {
            var medicineResponse = await _medicineService.GetByIdAsync(id);
            if (!medicineResponse.Success)
            {
                TempData["ErrorMessage"] = "Medicine not found.";
                return RedirectToAction(nameof(ListMedicines));
            }

            var updateVm = _mapper.Map<UpdateMedicineVM>(medicineResponse.Data);
            updateVm.IsDeleted = !updateVm.IsDeleted; 
            updateVm.Categories = await _medicineService.GetCategoriesSelectListAsync();

            var response = await _medicineService.UpdateAsync(updateVm);

            string statusText = updateVm.IsDeleted ? "deleted" : "restored";
            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Success ? $"Medicine {statusText} successfully." : "Failed to change medicine status.";

            return RedirectToAction(nameof(ListMedicines));
        }
    }
}
