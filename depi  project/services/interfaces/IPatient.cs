using depi__project.viewmodels.General;
using depi__project.viewmodels.Patient;

namespace depi__project.services.interfaces
{
    public interface IPatient
    {
        Task<ResponseStatus<IEnumerable<ResponsePatientVM>>> GetAllAsync(pagination pagination, string searchTerm = null, bool? isvalid = null);
        Task<ResponseStatus<ResponsePatientVM>> GetByIdAsync(int id);
        Task<ResponseStatus<ResponsePatientVM>> AddAsync(AddPatientVM patient);
        Task<ResponseStatus<ResponsePatientVM>> UpdateAsync(UpdatePatientVM patient);
        Task<ResponseStatus<bool>> DeleteAsync(int id);
        Task<ResponseStatus<bool>> ExistsAsync(int id);
        Task<int> GetCountAsync();
    }
}
