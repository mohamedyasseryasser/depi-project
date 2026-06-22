using AutoMapper;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.prescription;
using depi__project.viewmodels.prescriptionitems;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace depi__project.services.reporesity
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public PrescriptionRepository(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseStatus<IEnumerable<ResponsePrescriptionVM>>> GetAllAsync(
            pagination pagination,
            string? searchTerm = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponsePrescriptionVM>>();
            try
            {
                var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                var pageSize = pagination.PageSize <= 0 ? 10 : pagination.PageSize;

                IQueryable<Prescription> query = GetPrescriptionQuery();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p =>
                        p.Visit!.Appoinment!.Patient!.patientname.Contains(searchTerm) ||
                        p.Visit.Appoinment.Patient.phonenumber.Contains(searchTerm) ||
                        (p.notes != null && p.notes.Contains(searchTerm)));
                }

                var prescriptions = await query
                    .OrderByDescending(p => p.prescriptiondate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                response.Success = true;
                response.Data = _mapper.Map<IEnumerable<ResponsePrescriptionVM>>(prescriptions);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve prescriptions.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponsePrescriptionVM>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponsePrescriptionVM>();
            try
            {
                var prescription = await GetPrescriptionQuery()
                    .FirstOrDefaultAsync(p => p.prescriptionid == id);

                if (prescription == null)
                {
                    response.Success = false;
                    response.Message = "Prescription not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                response.Success = true;
                response.Data = _mapper.Map<ResponsePrescriptionVM>(prescription);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve prescription.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponsePrescriptionVM>> GetByVisitIdAsync(int visitId)
        {
            var response = new ResponseStatus<ResponsePrescriptionVM>();
            try
            {
                var prescription = await GetPrescriptionQuery()
                    .FirstOrDefaultAsync(p => p.visitid == visitId);

                if (prescription == null)
                {
                    response.Success = false;
                    response.Message = "No prescription found for this visit.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                response.Success = true;
                response.Data = _mapper.Map<ResponsePrescriptionVM>(prescription);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve prescription.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponsePrescriptionVM>> AddAsync(AddPrescriptionVM model)
        {
            var response = new ResponseStatus<ResponsePrescriptionVM>();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (model.items == null || !model.items.Any())
                {
                    response.Success = false;
                    response.Message = "At least one prescription item is required.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var visit = await _context.Visits
                    .Include(v => v.Prescription)
                    .FirstOrDefaultAsync(v => v.visitid == model.visitid);

                if (visit == null)
                {
                    response.Success = false;
                    response.Message = "Visit not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                if (visit.Prescription != null)
                {
                    response.Success = false;
                    response.Message = "This visit already has a prescription.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var medicineValidation = await ValidateMedicineIdsAsync(model.items.Select(i => i.mdeicineid));
                if (!medicineValidation.Success)
                {
                    return medicineValidation;
                }

                var prescription = _mapper.Map<Prescription>(model);
                prescription.items = model.items.Select(item => _mapper.Map<Prescriptionitems>(item)).ToList();

                await _context.Prescriptions.AddAsync(prescription);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.Success = true;
                response.Message = "Prescription created successfully.";
                response.Data = (await GetByIdAsync(prescription.prescriptionid)).Data;
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = "Failed to create prescription.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponsePrescriptionVM>> UpdateAsync(UpdatePrescriptionvm model)
        {
            var response = new ResponseStatus<ResponsePrescriptionVM>();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (model.items == null || !model.items.Any())
                {
                    response.Success = false;
                    response.Message = "At least one prescription item is required.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var prescription = await _context.Prescriptions
                    .Include(p => p.items)
                    .FirstOrDefaultAsync(p => p.prescriptionid == model.prescriptionid);

                if (prescription == null)
                {
                    response.Success = false;
                    response.Message = "Prescription not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var medicineValidation = await ValidateMedicineIdsAsync(model.items.Select(i => i.mdeicineid));
                if (!medicineValidation.Success)
                {
                    return medicineValidation;
                }

                prescription.prescriptiondate = model.prescriptiondate;
                prescription.notes = model.notes;
                prescription.visitid = model.visitid;

                _context.PrescriptionItems.RemoveRange(prescription.items);
                prescription.items = model.items.Select(item =>
                {
                    var entity = _mapper.Map<Prescriptionitems>(item);
                    entity.prescriptionid = prescription.prescriptionid;
                    return entity;
                }).ToList();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.Success = true;
                response.Message = "Prescription updated successfully.";
                response.Data = (await GetByIdAsync(prescription.prescriptionid)).Data;
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = "Failed to update prescription.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> DeleteAsync(int id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var prescription = await _context.Prescriptions
                    .Include(p => p.items)
                    .FirstOrDefaultAsync(p => p.prescriptionid == id);

                if (prescription == null)
                {
                    response.Success = false;
                    response.Data = false;
                    response.Message = "Prescription not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                _context.Prescriptions.Remove(prescription);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Prescription deleted successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Failed to delete prescription.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> ExistsAsync(int id)
        {
            var exists = await _context.Prescriptions.AnyAsync(p => p.prescriptionid == id);
            return new ResponseStatus<bool>(exists, exists ? null : "Prescription not found.", exists);
        }

        public async Task<ICollection<SelectListItem>> GetMedicinesForSelectAsync()
        {
            return await _context.Medicines
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Name)
                .Select(m => new SelectListItem
                {
                    Value = m.medicineId.ToString(),
                    Text = m.Name
                })
                .ToListAsync();
        }

        public async Task<ICollection<SelectListItem>> GetVisitsForSelectAsync(int? selectedVisitId = null)
        {
            var visits = await _context.Visits
                .Include(v => v.Prescription)
                .Include(v => v.Appoinment)
                    .ThenInclude(a => a!.Patient)
                .Where(v => v.Prescription == null || v.visitid == selectedVisitId)
                .OrderByDescending(v => v.visitdate)
                .ToListAsync();

            return visits.Select(v => new SelectListItem
            {
                Value = v.visitid.ToString(),
                Text = $"Visit #{v.visitid} - {v.Appoinment?.Patient?.patientname} ({v.visitdate:yyyy-MM-dd})",
                Selected = selectedVisitId.HasValue && v.visitid == selectedVisitId.Value
            }).ToList();
        }

        private IQueryable<Prescription> GetPrescriptionQuery()
        {
            return _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.items)
                    .ThenInclude(i => i.Medicine)
                .Include(p => p.Visit)
                    .ThenInclude(v => v!.Appoinment)
                        .ThenInclude(a => a!.Patient)
                .Include(p => p.Visit)
                    .ThenInclude(v => v!.Appoinment)
                        .ThenInclude(a => a!.Doctor)
                            .ThenInclude(d => d!.user);
        }

        private async Task<ResponseStatus<ResponsePrescriptionVM>> ValidateMedicineIdsAsync(IEnumerable<int> medicineIds)
        {
            var response = new ResponseStatus<ResponsePrescriptionVM>();
            var ids = medicineIds.Distinct().ToList();
            var existingCount = await _context.Medicines.CountAsync(m => ids.Contains(m.medicineId) && !m.IsDeleted);

            if (existingCount != ids.Count)
            {
                response.Success = false;
                response.Message = "One or more selected medicines are invalid.";
                response.Errors.Add(response.Message);
            }

            return response;
        }
    }
}
