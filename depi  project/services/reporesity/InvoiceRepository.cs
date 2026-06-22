using AutoMapper;
using depi__project.enums;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.invoice;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace depi__project.services.reporesity
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public InvoiceRepository(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public decimal CalculateFinalAmount(decimal totalAmount, decimal tax, decimal discount)
        {
            var final = totalAmount + tax - discount;
            return final < 0 ? 0 : Math.Round(final, 2);
        }

        public async Task<ResponseStatus<IEnumerable<ResponseInvoiceVM>>> GetAllAsync(
            pagination pagination,
            string? searchTerm = null,
            InvoiceStatus? status = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponseInvoiceVM>>();
            try
            {
                var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                var pageSize = pagination.PageSize <= 0 ? 10 : pagination.PageSize;

                IQueryable<Invoice> query = _context.Invoices
                    .AsNoTracking()
                    .Include(i => i.Visit)
                        .ThenInclude(v => v!.Appoinment)
                            .ThenInclude(a => a!.Patient)
                    .Include(i => i.Visit)
                        .ThenInclude(v => v!.Appoinment)
                            .ThenInclude(a => a!.Doctor)
                                .ThenInclude(d => d!.user);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(i =>
                        i.Visit!.Appoinment!.Patient!.patientname.Contains(searchTerm) ||
                        i.Visit.Appoinment.Patient.phonenumber.Contains(searchTerm) ||
                        i.InvoiceId.ToString().Contains(searchTerm));
                }

                if (status.HasValue)
                {
                    query = query.Where(i => i.Status == status.Value);
                }

                var invoices = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                response.Success = true;
                response.Data = _mapper.Map<IEnumerable<ResponseInvoiceVM>>(invoices);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve invoices.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseInvoiceVM>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponseInvoiceVM>();
            try
            {
                var invoice = await GetInvoiceQuery().FirstOrDefaultAsync(i => i.InvoiceId == id);
                if (invoice == null)
                {
                    response.Success = false;
                    response.Message = "Invoice not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                response.Success = true;
                response.Data = _mapper.Map<ResponseInvoiceVM>(invoice);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve invoice.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseInvoiceVM>> GetByVisitIdAsync(int visitId)
        {
            var response = new ResponseStatus<ResponseInvoiceVM>();
            try
            {
                var invoice = await GetInvoiceQuery().FirstOrDefaultAsync(i => i.VisitId == visitId);
                if (invoice == null)
                {
                    response.Success = false;
                    response.Message = "No invoice found for this visit.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                response.Success = true;
                response.Data = _mapper.Map<ResponseInvoiceVM>(invoice);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve invoice.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseInvoiceVM>> AddAsync(AddInvoiceVM model)
        {
            var response = new ResponseStatus<ResponseInvoiceVM>();
            try
            {
                var visit = await _context.Visits
                    .Include(v => v.Invoice)
                    .Include(v => v.Appoinment)
                    .FirstOrDefaultAsync(v => v.visitid == model.VisitId);

                if (visit == null)
                {
                    response.Success = false;
                    response.Message = "Visit not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                if (visit.Invoice != null)
                {
                    response.Success = false;
                    response.Message = "This visit already has an invoice.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                if (model.Discount > model.TotalAmount + model.Tax)
                {
                    response.Success = false;
                    response.Message = "Discount cannot exceed total amount plus tax.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var entity = _mapper.Map<Invoice>(model);
                entity.CreatedAt = DateTime.UtcNow;
                entity.FinalAmount = CalculateFinalAmount(model.TotalAmount, model.Tax, model.Discount);

                await _context.Invoices.AddAsync(entity);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Invoice created successfully.";
                response.Data = (await GetByIdAsync(entity.InvoiceId)).Data;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to create invoice.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseInvoiceVM>> UpdateAsync(UpdateInvoiceVM model)
        {
            var response = new ResponseStatus<ResponseInvoiceVM>();
            try
            {
                var entity = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == model.InvoiceId);
                if (entity == null)
                {
                    response.Success = false;
                    response.Message = "Invoice not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                if (model.Discount > model.TotalAmount + model.Tax)
                {
                    response.Success = false;
                    response.Message = "Discount cannot exceed total amount plus tax.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                _mapper.Map(model, entity);
                entity.FinalAmount = CalculateFinalAmount(model.TotalAmount, model.Tax, model.Discount);

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Invoice updated successfully.";
                response.Data = (await GetByIdAsync(entity.InvoiceId)).Data;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update invoice.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> DeleteAsync(int id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var entity = await _context.Invoices.FindAsync(id);
                if (entity == null)
                {
                    response.Success = false;
                    response.Data = false;
                    response.Message = "Invoice not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                _context.Invoices.Remove(entity);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Invoice deleted successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Failed to delete invoice.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> ExistsAsync(int id)
        {
            var exists = await _context.Invoices.AnyAsync(i => i.InvoiceId == id);
            return new ResponseStatus<bool>(exists, exists ? null : "Invoice not found.", exists);
        }

        private IQueryable<Invoice> GetInvoiceQuery()
        {
            return _context.Invoices
                .AsNoTracking()
                .Include(i => i.Visit)
                    .ThenInclude(v => v!.Appoinment)
                        .ThenInclude(a => a!.Patient)
                .Include(i => i.Visit)
                    .ThenInclude(v => v!.Appoinment)
                        .ThenInclude(a => a!.Doctor)
                            .ThenInclude(d => d!.user);
        }

        public async Task<ICollection<SelectListItem>> GetVisitsWithoutInvoiceAsync(int? selectedVisitId = null)
        {
            var visits = await _context.Visits
                .Include(v => v.Invoice)
                .Include(v => v.Appoinment)
                    .ThenInclude(a => a!.Patient)
                .Where(v => v.Invoice == null || v.visitid == selectedVisitId)
                .OrderByDescending(v => v.visitdate)
                .ToListAsync();

            return visits.Select(v => new SelectListItem
            {
                Value = v.visitid.ToString(),
                Text = $"Visit #{v.visitid} - {v.Appoinment?.Patient?.patientname} ({v.visitdate:yyyy-MM-dd})",
                Selected = selectedVisitId.HasValue && v.visitid == selectedVisitId.Value
            }).ToList();
        }

        public async Task<decimal?> GetSuggestedTotalForVisitAsync(int visitId)
        {
            var visit = await _context.Visits
                .Include(v => v.Appoinment)
                .FirstOrDefaultAsync(v => v.visitid == visitId);

            return visit?.Appoinment?.cost;
        }
    }
}
