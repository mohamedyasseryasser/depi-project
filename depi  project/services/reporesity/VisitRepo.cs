using AutoMapper;
using depi__project.enums;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.Visit;
using depi__project.viewmodels.VisitRepo;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depi__project.services.reporesity
{
    public class VisitRepo : IVisit
    {
        private readonly Context context;
        private readonly IMapper mapper;

        public VisitRepo(Context context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ResponseStatus<IEnumerable<ResponseVisitVM>>> GetAllAsync(pagination pagination, string? searchTerm = null, VisitStatus? status = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponseVisitVM>>();
            try
            {
                var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                var pageSize = pagination.PageSize <= 0 ? 10 : pagination.PageSize;

                IQueryable<Visit> query = context.Visits
                    .Include(v => v.Appoinment)
                        .ThenInclude(a => a.Patient)
                    .Include(v => v.Appoinment)
                        .ThenInclude(a => a.Doctor).ThenInclude(d => d.user)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(v =>
                        (v.notes != null && v.notes.Contains(searchTerm)) ||
                        (v.diagnosis != null && v.diagnosis.Contains(searchTerm)) ||
                        v.Appoinment.Patient.patientname.Contains(searchTerm) ||
                        v.Appoinment.PhoneNumber.Contains(searchTerm));
                }

                if (status.HasValue)
                {
                    query = query.Where(v => v.visitstatus == status.Value);
                }

                var visits = await query
                    .OrderByDescending(v => v.visitdate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                response.Success = true;
                response.Data = mapper.Map<IEnumerable<ResponseVisitVM>>(visits);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve visits.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseVisitVM>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponseVisitVM>();
            try
            {
                var visit = await context.Visits
                    .Include(v => v.Appoinment)
                        .ThenInclude(a => a.Patient)
                    .Include(v => v.Appoinment)
                        .ThenInclude(a => a.Doctor).ThenInclude(d => d.user)
                    .Include(v => v.Prescription)
                        .ThenInclude(p => p.items).ThenInclude(i => i.Medicine)
                    .Include(v => v.Invoice)
                    .FirstOrDefaultAsync(v => v.visitid == id);

                if (visit == null)
                {
                    response.Success = false;
                    response.Message = "Visit not found.";
                    response.Errors.Add("Visit not found.");
                    return response;
                }

                response.Success = true;
                response.Data = mapper.Map<ResponseVisitVM>(visit);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve visit details.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseVisitVM>> AddAsync(AddVisit visit)
        {
            var response = new ResponseStatus<ResponseVisitVM>();
            try
            {
                var appointment = await context.Appointments.FindAsync(visit.appoinmentid);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found.";
                    response.Errors.Add($"No appointment exists with ID: {visit.appoinmentid}");
                    return response;
                }

                // Check if a visit already exists for this appointment
                var existingVisit = await context.Visits.AnyAsync(v => v.appoinmentid == visit.appoinmentid);
                if (existingVisit)
                {
                    response.Success = false;
                    response.Message = "A visit already exists for this appointment.";
                    response.Errors.Add("Appointment already has an associated visit.");
                    return response;
                }

                var entity = mapper.Map<Visit>(visit);
                entity.visitdate = DateTime.Now;
                entity.visitstatus = VisitStatus.InProgress;

                appointment.status = AppointmentStatus.Confirmed; // Set appointment status to Confirmed/In Progress

                await context.Visits.AddAsync(entity);
                await context.SaveChangesAsync();

                var created = await context.Visits
                    .Include(v => v.Appoinment).ThenInclude(a => a.Patient)
                    .FirstOrDefaultAsync(v => v.visitid == entity.visitid);

                response.Success = true;
                response.Message = "Visit started successfully.";
                response.Data = mapper.Map<ResponseVisitVM>(created);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to add visit.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseVisitVM>> UpdateAsync(UpdateVisitVM visit)
        {
            var response = new ResponseStatus<ResponseVisitVM>();
            try
            {
                var existing = await context.Visits
                    .Include(v => v.Appoinment)
                    .FirstOrDefaultAsync(v => v.visitid == visit.visitid);

                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Visit not found.";
                    response.Errors.Add("Visit not found.");
                    return response;
                }

                existing.notes = visit.notes;
                existing.diagnosis = visit.diagnosis;
                existing.visitstatus = visit.visitstatus;

                if (visit.visitstatus == VisitStatus.Completed && existing.Appoinment != null)
                {
                    existing.Appoinment.status = AppointmentStatus.Completed;
                }

                await context.SaveChangesAsync();

                var updated = await context.Visits
                    .Include(v => v.Appoinment).ThenInclude(a => a.Patient)
                    .FirstOrDefaultAsync(v => v.visitid == existing.visitid);

                response.Success = true;
                response.Message = "Visit updated successfully.";
                response.Data = mapper.Map<ResponseVisitVM>(updated);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update visit.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> DeleteAsync(int id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var visit = await context.Visits.FindAsync(id);
                if (visit == null)
                {
                    response.Success = false;
                    response.Message = "Visit not found.";
                    response.Data = false;
                    return response;
                }

                context.Visits.Remove(visit);
                await context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Visit deleted successfully.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to delete visit.";
                response.Errors.Add(ex.Message);
                response.Data = false;
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> ExistsAsync(int id)
        {
            var exists = await context.Visits.AnyAsync(v => v.visitid == id);
            return new ResponseStatus<bool>(exists, exists ? null : "Visit not found.", exists);
        }

        public async Task<int> GetCountAsync()
        {
            return await context.Visits.CountAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetPendingAppointmentsForVisitAsync()
        {
            return await context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor).ThenInclude(d => d.user)
                .Where(a => a.Visit == null && (a.status == AppointmentStatus.Pending || a.status == AppointmentStatus.Confirmed))
                .Select(a => new SelectListItem
                {
                    Value = a.appoimentid.ToString(),
                    Text = $"Appt #{a.appoimentid} - {a.Patient.patientname} (Phone: {a.PhoneNumber}) with Dr. {a.Doctor.user.UserName} at {a.Appoinmentdate:yyyy-MM-dd}"
                })
                .ToListAsync();
        }
    }
}
