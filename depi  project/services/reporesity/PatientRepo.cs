using AutoMapper;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.Patient;
using Microsoft.EntityFrameworkCore;

namespace depi__project.services.reporesity
{
    public class PatientRepo : IPatient
    {
        private readonly Context context;
        private readonly IMapper mapper;

        public PatientRepo(Context context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ResponseStatus<IEnumerable<ResponsePatientVM>>> GetAllAsync(pagination pagination, string searchTerm = null, bool? isvalid = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponsePatientVM>>();
            try
            {
                var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                var pageSize = pagination.PageSize <= 0 ? 10 : pagination.PageSize;

                IQueryable<Patient> query = context.Patients.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p =>
                        p.patientname.Contains(searchTerm) ||
                        p.phonenumber.Contains(searchTerm) ||
                        (p.nationalid.HasValue && p.nationalid.Value.ToString().Contains(searchTerm)));
                }

                if (isvalid.HasValue)
                {
                    query = query.Where(p => p.isvalid == isvalid.Value);
                }

                var patients = await query
                    .OrderByDescending(p => p.patientid)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                response.Success = true;
                response.Data = mapper.Map<IEnumerable<ResponsePatientVM>>(patients);
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Failed to retrieve patients.";
                response.Errors.Add(response.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponsePatientVM>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponsePatientVM>();
            try
            {
                var patient = await context.Patients
                    .AsNoTracking()
                    .Include(p => p.appoinments)
                        .ThenInclude(a => a.Doctor)
                            .ThenInclude(d => d.user)
                    .Include(p => p.appoinments)
                        .ThenInclude(a => a.Visit)
                            .ThenInclude(v => v.Prescription)
                                .ThenInclude(p => p.items)
                                    .ThenInclude(i => i.Medicine)
                    .Include(p => p.appoinments)
                        .ThenInclude(a => a.Visit)
                            .ThenInclude(v => v.Invoice)
                    .FirstOrDefaultAsync(p => p.patientid == id);

                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                response.Success = true;
                response.Data = MapPatientDetails(patient);
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Failed to retrieve patient.";
                response.Errors.Add(response.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponsePatientVM>> AddAsync(AddPatientVM patient)
        {
            var response = new ResponseStatus<ResponsePatientVM>();
            try
            {
                var entity = mapper.Map<Patient>(patient);
                entity.isvalid = true;

                await context.Patients.AddAsync(entity);
                await context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Patient added successfully.";
                response.Data = mapper.Map<ResponsePatientVM>(entity);
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Failed to add patient.";
                response.Errors.Add(response.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponsePatientVM>> UpdateAsync(UpdatePatientVM patient)
        {
            var response = new ResponseStatus<ResponsePatientVM>();
            try
            {
                var existing = await context.Patients.FindAsync(patient.patientid);
                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found.";
                    response.Errors.Add(response.Message);
                    return response;
                }

                existing.patientname = patient.patientname;
                existing.phonenumber = patient.phonenumber;
                existing.Gender = patient.Gender;
                existing.datebirth = patient.datebirth;
                existing.nationalid = patient.nationalid;
                existing.isvalid = patient.isvalid;

                await context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Patient updated successfully.";
                response.Data = mapper.Map<ResponsePatientVM>(existing);
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Failed to update patient.";
                response.Errors.Add(response.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> DeleteAsync(int id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var patient = await context.Patients.FindAsync(id);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found.";
                    response.Errors.Add(response.Message);
                    response.Data = false;
                    return response;
                }

                patient.isvalid = false;
                await context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Patient deactivated successfully.";
                response.Data = true;
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Failed to deactivate patient.";
                response.Errors.Add(response.Message);
                response.Data = false;
                return response;
            }
        }

        public async Task<ResponseStatus<bool>> ExistsAsync(int id)
        {
            var exists = await context.Patients.AnyAsync(p => p.patientid == id && p.isvalid);
            return new ResponseStatus<bool>(exists, exists ? null : "Patient not found.", exists);
        }

        public async Task<int> GetCountAsync()
        {
            return await context.Patients.CountAsync();
        }

        private static ResponsePatientVM MapPatientDetails(Patient patient)
        {
            var visits = patient.appoinments
                .Where(a => a.Visit != null)
                .Select(a => a.Visit!)
                .ToList();

            return new ResponsePatientVM
            {
                patientid = patient.patientid,
                patientname = patient.patientname,
                phonenumber = patient.phonenumber,
                Gender = patient.Gender,
                datebirth = patient.datebirth,
                nationalid = patient.nationalid,
                isvalid = patient.isvalid,
                Appointments = patient.appoinments.Select(a => new AppointmentResponseVM
                {
                    appoimentid = a.appoimentid,
                    Appoinmentdate = a.Appoinmentdate,
                    startat = a.startat,
                    endat = a.endat,
                    notes = a.notes,
                    status = a.status,
                    DoctorName = a.Doctor?.user?.UserName
                }).ToList(),
                Visits = visits.Select(v => new VisitResponseVM
                {
                    visitid = v.visitid,
                    visitdate = v.visitdate,
                    diagnosis = v.diagnosis,
                    notes = v.notes,
                    visitstatus = v.visitstatus
                }).ToList(),
                Prescriptions = visits
                    .Where(v => v.Prescription != null)
                    .Select(v => new PrescriptionResponseVM
                    {
                        prescriptionid = v.Prescription!.prescriptionid,
                        prescriptiondate = v.Prescription.prescriptiondate,
                        notes = v.Prescription.notes,
                        Medicines = v.Prescription.items
                            .Where(i => i.Medicine != null)
                            .Select(i => i.Medicine.Name)
                            .ToList()
                    }).ToList(),
                Invoices = visits
                    .Where(v => v.Invoice != null)
                    .Select(v => new InvoiceResponseVM
                    {
                        InvoiceId = v.Invoice!.InvoiceId,
                        FinalAmount = v.Invoice.FinalAmount,
                        CreatedAt = v.Invoice.CreatedAt,
                        Status = v.Invoice.Status
                    }).ToList()
            };
        }
    }
}
