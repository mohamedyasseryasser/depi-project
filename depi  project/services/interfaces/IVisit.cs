using depi__project.enums;
using depi__project.viewmodels.Visit;
using depi__project.viewmodels.VisitRepo;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace depi__project.services.interfaces
{
    public interface IVisit
    {
        Task<ResponseStatus<IEnumerable<ResponseVisitVM>>> GetAllAsync(pagination pagination, string? searchTerm = null, VisitStatus? status = null);
        Task<ResponseStatus<ResponseVisitVM>> GetByIdAsync(int id);
        Task<ResponseStatus<ResponseVisitVM>> AddAsync(AddVisit visit);
        Task<ResponseStatus<ResponseVisitVM>> UpdateAsync(UpdateVisitVM visit);
        Task<ResponseStatus<bool>> DeleteAsync(int id);
        Task<ResponseStatus<bool>> ExistsAsync(int id);
        Task<int> GetCountAsync();
        Task<IEnumerable<SelectListItem>> GetPendingAppointmentsForVisitAsync();
    }
}
