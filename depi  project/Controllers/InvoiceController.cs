using AutoMapper;
using depi__project.enums;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.invoice;
using Microsoft.AspNetCore.Mvc;

namespace depi__project.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public InvoiceController(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, InvoiceStatus? status = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            var response = await _invoiceRepository.GetAllAsync(pagination, searchTerm, status);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve invoices.";
                return View(new List<ResponseInvoiceVM>());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _invoiceRepository.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Invoice not found.";
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
        public Task<IActionResult> CreateInvoice(int visitId) => BuildCreateView(visitId);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddInvoiceVM model)
        {
            model.FinalAmount = _invoiceRepository.CalculateFinalAmount(model.TotalAmount, model.Tax, model.Discount);

            if (!ModelState.IsValid)
            {
                model.Visits = await _invoiceRepository.GetVisitsWithoutInvoiceAsync(model.VisitId);
                return View(model);
            }

            var response = await _invoiceRepository.AddAsync(model);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                model.Visits = await _invoiceRepository.GetVisitsWithoutInvoiceAsync(model.VisitId);
                TempData["ErrorMessage"] = response.Message ?? "Failed to create invoice.";
                return View(model);
            }

            TempData["SuccessMessage"] = response.Message ?? "Invoice created successfully.";
            return RedirectToAction(nameof(Details), new { id = response.Data!.InvoiceId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _invoiceRepository.GetByIdAsync(id);
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Invoice not found.";
                return RedirectToAction(nameof(Index));
            }

            var vm = _mapper.Map<UpdateInvoiceVM>(response.Data);
            vm.Visits = await _invoiceRepository.GetVisitsWithoutInvoiceAsync(vm.VisitId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateInvoiceVM model)
        {
            model.FinalAmount = _invoiceRepository.CalculateFinalAmount(model.TotalAmount, model.Tax, model.Discount);

            if (!ModelState.IsValid)
            {
                model.Visits = await _invoiceRepository.GetVisitsWithoutInvoiceAsync(model.VisitId);
                return View(model);
            }

            var response = await _invoiceRepository.UpdateAsync(model);
            if (!response.Success)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                model.Visits = await _invoiceRepository.GetVisitsWithoutInvoiceAsync(model.VisitId);
                TempData["ErrorMessage"] = response.Message ?? "Failed to update invoice.";
                return View(model);
            }

            TempData["SuccessMessage"] = response.Message ?? "Invoice updated successfully.";
            return RedirectToAction(nameof(Details), new { id = response.Data!.InvoiceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _invoiceRepository.DeleteAsync(id);
            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Message ?? (response.Success ? "Invoice deleted successfully." : "Failed to delete invoice.");

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> BuildCreateView(int? visitId)
        {
            var vm = new AddInvoiceVM
            {
                VisitId = visitId ?? 0,
                Status = InvoiceStatus.Pending,
                Visits = await _invoiceRepository.GetVisitsWithoutInvoiceAsync(visitId)
            };

            if (visitId.HasValue && visitId.Value > 0)
            {
                var suggestedTotal = await _invoiceRepository.GetSuggestedTotalForVisitAsync(visitId.Value);
                if (suggestedTotal.HasValue)
                {
                    vm.TotalAmount = suggestedTotal.Value;
                    vm.FinalAmount = _invoiceRepository.CalculateFinalAmount(vm.TotalAmount, vm.Tax, vm.Discount);
                }

                vm.PatientName = vm.Visits.FirstOrDefault(v => v.Selected)?.Text;
            }

            return View("Create", vm);
        }
    }
}
