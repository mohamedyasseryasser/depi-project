using AutoMapper;
using depi__project.enums;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.Appoinment;
using depi__project.viewmodels.General;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace depi__project.services.reporesity
{
    public class AppoinmentRepo : IAppoinment
    {
        private readonly Context context;
        private readonly IMapper mapper;

        public AppoinmentRepo(Context context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ResponseStatus<IEnumerable<ResponseAppoimentVM>>> GetAllAsync(pagination pagination, string? searchTerm = null, string? phone = null, DateTime? date = null, AppointmentStatus? status = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponseAppoimentVM>>();
            try
            {
                var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                var pageSize = pagination.PageSize <= 0 ? 10 : pagination.PageSize;

                IQueryable<Appoinment> query = context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor).ThenInclude(d => d.user)
                    .Include(a => a.resptionist).ThenInclude(r => r.user)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(a =>
                        a.PhoneNumber.Contains(searchTerm) ||
                        (a.notes != null && a.notes.Contains(searchTerm)) ||
                        a.Patient.patientname.Contains(searchTerm) ||
                        a.Doctor.user.UserName.Contains(searchTerm));
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    query = query.Where(a => a.PhoneNumber.Contains(phone));
                }

                if (date.HasValue)
                {
                    query = query.Where(a => a.Appoinmentdate.Date == date.Value.Date);
                }

                if (status.HasValue)
                {
                    query = query.Where(a => a.status == status.Value);
                }

                var appointments = await query
                    .OrderByDescending(a => a.Appoinmentdate)
                    .ThenByDescending(a => a.startat)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                response.Success = true;
                response.Data = mapper.Map<IEnumerable<ResponseAppoimentVM>>(appointments);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve appointments.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseAppoimentVM>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponseAppoimentVM>();
            try
            {
                var appointment = await context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor).ThenInclude(d => d.user)
                    .Include(a => a.resptionist).ThenInclude(r => r.user)
                    .Include(a => a.Visit)
                    .FirstOrDefaultAsync(a => a.appoimentid == id);

                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found.";
                    response.Errors.Add("Appointment not found.");
                    return response;
                }

                response.Success = true;
                response.Data = mapper.Map<ResponseAppoimentVM>(appointment);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve appointment details.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseAppoimentVM>> AddAsync(AddAppoinmentVM appointment)
        {
            var response = new ResponseStatus<ResponseAppoimentVM>();
            try
            {
                var patient = await context.Patients.FirstOrDefaultAsync(p => p.phonenumber == appointment.PhoneNumber && p.isvalid);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found. Please register the patient first.";
                    response.Errors.Add($"No active patient exists with phone number: {appointment.PhoneNumber}. Please register the patient before booking.");
                    return response;
                }

                var entity = mapper.Map<Appoinment>(appointment);
                entity.patientid = patient.patientid;
                entity.status = AppointmentStatus.Pending;
                entity.updateat = DateTime.Now;

                await context.Appointments.AddAsync(entity);
                await context.SaveChangesAsync();

                var created = await context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor).ThenInclude(d => d.user)
                    .Include(a => a.resptionist).ThenInclude(r => r.user)
                    .FirstOrDefaultAsync(a => a.appoimentid == entity.appoimentid);

                response.Success = true;
                response.Message = "Appointment booked successfully.";
                response.Data = mapper.Map<ResponseAppoimentVM>(created);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to add appointment.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseAppoimentVM>> UpdateAsync(UpdateAppoinment appointment)
        {
            var response = new ResponseStatus<ResponseAppoimentVM>();
            try
            {
                var existing = await context.Appointments.FindAsync(appointment.AppoinmentId);
                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found.";
                    response.Errors.Add("Appointment not found.");
                    return response;
                }

                var patient = await context.Patients.FirstOrDefaultAsync(p => p.phonenumber == appointment.PhoneNumber && p.isvalid);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found for the given phone number.";
                    response.Errors.Add($"No active patient exists with phone number: {appointment.PhoneNumber}.");
                    return response;
                }

                existing.PhoneNumber = appointment.PhoneNumber;
                existing.Appoinmentdate = appointment.Appoinmentdate;
                existing.startat = appointment.startat;
                existing.endat = appointment.endat;
                existing.cost = appointment.cost;
                existing.type = appointment.type;
                existing.resptionistidid = appointment.resptionistidid;
                existing.notes = appointment.notes;
                existing.doctorid = appointment.doctorid;
                existing.patientid = patient.patientid;
                existing.updateat = DateTime.Now;

                await context.SaveChangesAsync();

                var updated = await context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor).ThenInclude(d => d.user)
                    .Include(a => a.resptionist).ThenInclude(r => r.user)
                    .FirstOrDefaultAsync(a => a.appoimentid == existing.appoimentid);

                response.Success = true;
                response.Message = "Appointment updated successfully.";
                response.Data = mapper.Map<ResponseAppoimentVM>(updated);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update appointment.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> UpdateStatusAsync(UpdateAppoinmentStateVM vm)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var existing = await context.Appointments.FindAsync(vm.AppoinmentId);
                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found.";
                    response.Data = false;
                    return response;
                }

                existing.status = vm.Status;
                existing.updateat = DateTime.Now;
                await context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Appointment status updated successfully.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update appointment status.";
                response.Errors.Add(ex.Message);
                response.Data = false;
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> DeleteAsync(int id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var appointment = await context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found.";
                    response.Data = false;
                    return response;
                }

                context.Appointments.Remove(appointment);
                await context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Appointment deleted successfully.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to delete appointment.";
                response.Errors.Add(ex.Message);
                response.Data = false;
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> ExistsAsync(int id)
        {
            var exists = await context.Appointments.AnyAsync(a => a.appoimentid == id);
            return new ResponseStatus<bool>(exists, exists ? null : "Appointment not found.", exists);
        }

        public async Task<int> GetCountAsync()
        {
            return await context.Appointments.CountAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetDoctorsSelectListAsync()
        {
            return await context.Doctors
                .Include(d => d.user)
                .Select(d => new SelectListItem
                {
                    Value = d.DoctorId.ToString(),
                    Text = $"{d.user.UserName} ({d.Specialization})"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetReceptionistsSelectListAsync()
        {
            return await context.resptionists
                .Include(r => r.user)
                .Select(r => new SelectListItem
                {
                    Value = r.resptionistid.ToString(),
                    Text = r.user.UserName
                })
                .ToListAsync();
        }
    }
}
