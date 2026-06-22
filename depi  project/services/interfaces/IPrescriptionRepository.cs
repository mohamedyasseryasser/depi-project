using depi__project.viewmodels.General;
using depi__project.viewmodels.prescription;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace depi__project.services.interfaces
{
    public interface IPrescriptionRepository
    {
        Task<ResponseStatus<IEnumerable<ResponsePrescriptionVM>>> GetAllAsync(pagination pagination, string? searchTerm = null);
        Task<ResponseStatus<ResponsePrescriptionVM>> GetByIdAsync(int id);
        Task<ResponseStatus<ResponsePrescriptionVM>> GetByVisitIdAsync(int visitId);
        Task<ResponseStatus<ResponsePrescriptionVM>> AddAsync(AddPrescriptionVM model);
        Task<ResponseStatus<ResponsePrescriptionVM>> UpdateAsync(UpdatePrescriptionvm model);
        Task<ResponseStatus<bool>> DeleteAsync(int id);
        Task<ResponseStatus<bool>> ExistsAsync(int id);
        Task<ICollection<SelectListItem>> GetMedicinesForSelectAsync();
        Task<ICollection<SelectListItem>> GetVisitsForSelectAsync(int? selectedVisitId = null);
    }
}
