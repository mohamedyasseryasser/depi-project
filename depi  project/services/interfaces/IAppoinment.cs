using depi__project.enums;
using depi__project.viewmodels.Appoinment;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace depi__project.services.interfaces
{
    public interface IAppoinment
    {
        Task<ResponseStatus<IEnumerable<ResponseAppoimentVM>>> GetAllAsync(pagination pagination, string? searchTerm = null, string? phone = null, DateTime? date = null, AppointmentStatus? status = null);
        Task<ResponseStatus<ResponseAppoimentVM>> GetByIdAsync(int id);
        Task<ResponseStatus<ResponseAppoimentVM>> AddAsync(AddAppoinmentVM appointment);
        Task<ResponseStatus<ResponseAppoimentVM>> UpdateAsync(UpdateAppoinment appointment);
        Task<ResponseStatus<bool>> UpdateStatusAsync(UpdateAppoinmentStateVM vm);
        Task<ResponseStatus<bool>> DeleteAsync(int id);
        Task<ResponseStatus<bool>> ExistsAsync(int id);
        Task<int> GetCountAsync();
        Task<IEnumerable<SelectListItem>> GetDoctorsSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetReceptionistsSelectListAsync();
    }
}
