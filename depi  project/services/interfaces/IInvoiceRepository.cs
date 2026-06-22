using depi__project.enums;
using depi__project.viewmodels.General;
using depi__project.viewmodels.invoice;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace depi__project.services.interfaces
{
    public interface IInvoiceRepository
    {
        Task<ResponseStatus<IEnumerable<ResponseInvoiceVM>>> GetAllAsync(pagination pagination, string? searchTerm = null, InvoiceStatus? status = null);
        Task<ResponseStatus<ResponseInvoiceVM>> GetByIdAsync(int id);
        Task<ResponseStatus<ResponseInvoiceVM>> GetByVisitIdAsync(int visitId);
        Task<ResponseStatus<ResponseInvoiceVM>> AddAsync(AddInvoiceVM model);
        Task<ResponseStatus<ResponseInvoiceVM>> UpdateAsync(UpdateInvoiceVM model);
        Task<ResponseStatus<bool>> DeleteAsync(int id);
        Task<ResponseStatus<bool>> ExistsAsync(int id);
        decimal CalculateFinalAmount(decimal totalAmount, decimal tax, decimal discount);
        Task<ICollection<SelectListItem>> GetVisitsWithoutInvoiceAsync(int? selectedVisitId = null);
        Task<decimal?> GetSuggestedTotalForVisitAsync(int visitId);
    }
}
