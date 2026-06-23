using depi__project.Models;
using depi__project.viewmodels.medicine;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace depi__project.services.interfaces
{
    public interface IMedicineRepository
    {
        Task<ResponseStatus<IEnumerable<ResponseMedicineVM>>> GetAllAsync(pagination pg, bool? isDeleted = null, string name = null, int? cat_id = null);
        Task<ResponseStatus<ResponseMedicineVM>> GetByIdAsync(int id);
        Task<ResponseStatus<ResponseMedicineVM>> AddAsync(AddMedicineVM vm);
        Task<ResponseStatus<ResponseMedicineVM>> UpdateAsync(UpdateMedicineVM vm);
        Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync();
    }
}
