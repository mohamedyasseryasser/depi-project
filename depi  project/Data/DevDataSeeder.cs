using depi__project.enums;
using depi__project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace depi__project.Data
{
    public static class DevDataSeeder
    {
        public static async Task SeedAsync(Context context, UserManager<Aplicationuser> userManager)
        {
            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                return;
            }

            await SeedMedicinesAsync(context, admin);
            await SeedVisitForPrescriptionAsync(context, userManager, admin);
        }

        private static async Task SeedMedicinesAsync(Context context, Aplicationuser admin)
        {
            if (await context.Medicines.AnyAsync())
            {
                return;
            }

            var category = await context.categories.FirstOrDefaultAsync();
            if (category == null)
            {
                category = new Category
                {
                    cat_name = "General",
                    cat_description = "General medicines",
                    isactive = true,
                    AddedBy = admin.UserName,
                    user_id = admin.Id
                };
                context.categories.Add(category);
                await context.SaveChangesAsync();
            }

            var medicines = new[]
            {
                new Medicine { Name = "Paracetamol 500mg", UnitPrice = 15m, StockQuantity = 100, SupplierName = "PharmaCo", RecordLevel = 10, ExpiryDate = DateTime.UtcNow.AddYears(2), CategoryName = category.cat_name, cat_id = category.cat_id, AddedBy = admin.UserName!, user_id = admin.Id, CreatedAt = DateTime.UtcNow },
                new Medicine { Name = "Amoxicillin 250mg", UnitPrice = 45m, StockQuantity = 80, SupplierName = "PharmaCo", RecordLevel = 10, ExpiryDate = DateTime.UtcNow.AddYears(2), CategoryName = category.cat_name, cat_id = category.cat_id, AddedBy = admin.UserName!, user_id = admin.Id, CreatedAt = DateTime.UtcNow },
                new Medicine { Name = "Ibuprofen 400mg", UnitPrice = 25m, StockQuantity = 120, SupplierName = "MedSupply", RecordLevel = 10, ExpiryDate = DateTime.UtcNow.AddYears(2), CategoryName = category.cat_name, cat_id = category.cat_id, AddedBy = admin.UserName!, user_id = admin.Id, CreatedAt = DateTime.UtcNow },
                new Medicine { Name = "Omeprazole 20mg", UnitPrice = 35m, StockQuantity = 60, SupplierName = "MedSupply", RecordLevel = 10, ExpiryDate = DateTime.UtcNow.AddYears(2), CategoryName = category.cat_name, cat_id = category.cat_id, AddedBy = admin.UserName!, user_id = admin.Id, CreatedAt = DateTime.UtcNow }
            };

            context.Medicines.AddRange(medicines);
            await context.SaveChangesAsync();
        }

        private static async Task SeedVisitForPrescriptionAsync(Context context, UserManager<Aplicationuser> userManager, Aplicationuser admin)
        {
            if (await context.Visits.AnyAsync())
            {
                return;
            }

            var department = await context.Departments.FirstOrDefaultAsync(d => d.isactive)
                ?? await context.Departments.FirstOrDefaultAsync();

            if (department == null)
            {
                department = new Department
                {
                    Name = "General Medicine",
                    FloorNumber = 1,
                    Phone = "01000000001",
                    description = "General outpatient clinic",
                    isactive = true,
                    createdat = DateTime.UtcNow,
                    userid = admin.Id
                };
                context.Departments.Add(department);
                await context.SaveChangesAsync();
            }

            var patient = new Patient
            {
                patientname = "Ahmed Hassan",
                phonenumber = "01098765432",
                Gender = Gender.Male,
                datebirth = new DateTime(1990, 5, 15),
                nationalid = 29005151234567,
                isvalid = true
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var doctorUser = await EnsureUserAsync(userManager, "doctor1", "doctor1@clinic.local", "01011111111", "Doctor@123", "Doctor");
            var receptionistUser = await EnsureUserAsync(userManager, "reception1", "reception1@clinic.local", "01022222222", "Reception@123", "Receptionist");

            var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.userid == doctorUser.Id);
            if (doctor == null)
            {
                doctor = new Doctor
                {
                    userid = doctorUser.Id,
                    DepartmentId = department.DepartmentId,
                    Specialization = "General Medicine",
                    hiredate = DateTime.UtcNow.AddYears(-1),
                    salary = 8000m,
                    status = userstatus.active
                };
                context.Doctors.Add(doctor);
                await context.SaveChangesAsync();
            }

            var receptionist = await context.resptionists.FirstOrDefaultAsync(r => r.userid == receptionistUser.Id);
            if (receptionist == null)
            {
                receptionist = new resptionist
                {
                    userid = receptionistUser.Id,
                    salary = 4000m,
                    status = userstatus.active,
                    hiredate = DateTime.UtcNow.AddYears(-1)
                };
                context.resptionists.Add(receptionist);
                await context.SaveChangesAsync();
            }

            var appointmentDate = DateTime.Today;
            var appointment = new Appoinment
            {
                patientid = patient.patientid,
                doctorid = doctor.DoctorId,
                resptionistidid = receptionist.resptionistid,
                PhoneNumber = patient.phonenumber,
                Appoinmentdate = appointmentDate,
                startat = appointmentDate.AddHours(10),
                endat = appointmentDate.AddHours(10).AddMinutes(30),
                updateat = DateTime.UtcNow,
                notes = "Demo appointment for prescription testing",
                cost = 200m,
                type = typeofappoinment.New,
                status = AppointmentStatus.Confirmed
            };
            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();

            context.Visits.Add(new Visit
            {
                appoinmentid = appointment.appoimentid,
                visitdate = appointmentDate,
                diagnosis = "Upper respiratory infection",
                notes = "Demo visit - ready for prescription",
                visitstatus = VisitStatus.InProgress
            });
            await context.SaveChangesAsync();
        }

        private static async Task<Aplicationuser> EnsureUserAsync(
            UserManager<Aplicationuser> userManager,
            string userName,
            string email,
            string phone,
            string password,
            string role)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return user;
            }

            user = new Aplicationuser
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phone,
                address = "Clinic HQ",
                Gender = Gender.Male,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }
    }
}
