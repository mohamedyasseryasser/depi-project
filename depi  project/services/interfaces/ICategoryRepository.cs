using depi__project.Models;
using depi__project.viewmodels.category;
using depi__project.viewmodels.General;

namespace depi__project.services.interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<ResponseStatus<IEnumerable<ResponseCategoryVM>>> GetAllAsync(pagination pg, bool? isactive = null, string name = null);
        Task<ResponseStatus<ResponseCategoryVM>> GetByIdAsync(int id);
        Task<ResponseStatus<ResponseCategoryVM>> AddAsync(AddCategoryVM vm);
        Task<ResponseStatus<ResponseCategoryVM>> UpdateAsync(UpdateCategoryVM vm);
    }
}
