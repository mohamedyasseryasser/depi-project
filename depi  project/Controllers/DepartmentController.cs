using System.Security.Claims;
using AutoMapper;
using depi__project.services.interfaces;
using depi__project.viewmodels.departmentvm;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc;

namespace depi__project.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartment departmentService;
        private readonly IMapper mapper;

        public DepartmentController(IDepartment departmentService, IMapper mapper)
        {
            this.departmentService = departmentService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> ListDepartments(int pageNumber = 1, int pageSize = 10, string name = null, string phone = null, bool? isactive = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response = await departmentService.GetAllAsync(pagination, phone, isactive, name);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.Name = name;
            ViewBag.Phone = phone;
            ViewBag.IsActive = isactive;

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve departments.";
                return View(new List<ResponseDeptVm>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> DepartmentDetails(int id)
        {
            var response = await departmentService.GetByIdWithDoctorsAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Department not found.";
                return RedirectToAction(nameof(ListDepartments));
            }

            return View(response.Data);
        }

        public IActionResult AddDepartment()
        {
            return View(new AddDeptVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddDepartment(AddDeptVm vm)
        {
            vm.userid ??= User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return View("AddDepartment", vm);
            }

            var response = await departmentService.AddAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                TempData["ErrorMessage"] = response.Message ?? "Failed to add department.";
                return View("AddDepartment", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Department added successfully.";
            return RedirectToAction(nameof(DepartmentDetails), new { id = response.Data.DepartmentId });
        }

        public async Task<IActionResult> UpdateDepartment(int id)
        {
            var response = await departmentService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Department not found.";
                return RedirectToAction(nameof(ListDepartments));
            }

            var vm = mapper.Map<UpdateDertVm>(response.Data);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdateDepartment(UpdateDertVm vm)
        {
            vm.userid ??= User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                return View("UpdateDepartment", vm);
            }

            var response = await departmentService.UpdateAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                TempData["ErrorMessage"] = response.Message ?? "Failed to update department.";
                return View("UpdateDepartment", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Department updated successfully.";
            return RedirectToAction(nameof(DepartmentDetails), new { id = response.Data.DepartmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDepartment(int id)
        {
            var response = await departmentService.DeleteAsync(id);
            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Message ?? (response.Success ? "Department deactivated successfully." : "Failed to deactivate department.");

            return RedirectToAction(nameof(ListDepartments));
        }
    }
}
