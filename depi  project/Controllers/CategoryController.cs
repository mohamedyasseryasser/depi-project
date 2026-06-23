using System.Security.Claims;
using AutoMapper;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.category;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc;

namespace depi__project.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper          = mapper;
        }

        public async Task<IActionResult> ListCategories(
            int pageNumber = 1, int pageSize = 10,
            string name = null, bool? isactive = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response   = await _categoryService.GetAllAsync(pagination, isactive, name);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize   = pageSize;
            ViewBag.Name       = name;
            ViewBag.IsActive   = isactive;

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve categories.";
                return View(new List<ResponseCategoryVM>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> CategoryDetails(int id)
        {
            var response = await _categoryService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Category not found.";
                return RedirectToAction(nameof(ListCategories));
            }

            return View(response.Data);
        }

        public IActionResult AddCategory()
        {
            return View(new AddCategoryVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddCategory(AddCategoryVM vm)
        {
            vm.user_id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            if (!ModelState.IsValid)
                return View("AddCategory", vm);

            var response = await _categoryService.AddAsync(vm);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to add category.";
                return View("AddCategory", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Category added successfully.";
            return RedirectToAction(nameof(CategoryDetails), new { id = response.Data.cat_id });
        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            var response = await _categoryService.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Category not found.";
                return RedirectToAction(nameof(ListCategories));
            }

            var vm = _mapper.Map<UpdateCategoryVM>(response.Data);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdateCategory(UpdateCategoryVM vm)
        {

            if (!ModelState.IsValid)
                return View("UpdateCategory", vm);

            var response = await _categoryService.UpdateAsync(vm);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                    ModelState.AddModelError(string.Empty, error);

                TempData["ErrorMessage"] = response.Message ?? "Failed to update category.";
                return View("UpdateCategory", vm);
            }

            TempData["SuccessMessage"] = response.Message ?? "Category updated successfully.";
            return RedirectToAction(nameof(CategoryDetails), new { id = response.Data.cat_id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCategory(int id)
        {
            var categoryResponse = await _categoryService.GetByIdAsync(id);
            if (!categoryResponse.Success)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(ListCategories));
            }

            var updateVm = _mapper.Map<UpdateCategoryVM>(categoryResponse.Data);
            updateVm.isactive = !updateVm.isactive;

            var response = await _categoryService.UpdateAsync(updateVm);

            if (response.Success)
            {
                string statusText = updateVm.isactive ? "activated" : "deactivated";
                TempData["SuccessMessage"] = $"Category {statusText} successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to change category status.";
            }

            return RedirectToAction(nameof(ListCategories));
        }
    }
}
