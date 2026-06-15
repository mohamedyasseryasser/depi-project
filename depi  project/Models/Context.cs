using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using depi__project.Models;
using System.Data;
using System.Reflection.Emit;

namespace depi__project.Models
{
    public class Context:IdentityDbContext<Aplicationuser>
    {
        public Context(DbContextOptions<Context> options):base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //applicationuser --> admin (1:1)
            builder.Entity<Admin>()
                   .HasOne(a => a.user)
                   .WithOne()
                   .HasForeignKey<Admin>(a => a.userid)
                   .OnDelete(DeleteBehavior.Cascade);
            //doctor --> appoinment (1:m)
            builder.Entity<Appoinment>().
                HasOne(a => a.Doctor).
                WithMany(d => d.appoinments).
                HasForeignKey(a => a.doctorid)
                .OnDelete(DeleteBehavior.Restrict);
            //department --> applicationuser (1:m)
            builder.Entity<Department>()
            .HasOne(d => d.user)
            .WithMany()
            .HasForeignKey(d => d.userid)
            .OnDelete(DeleteBehavior.Restrict);
            // Doctor → ApplicationUser (1:1)
            builder.Entity<Doctor>()
         .HasOne(d => d.user)
        .WithOne()
        .HasForeignKey<Doctor>(d => d.userid)
        .OnDelete(DeleteBehavior.Cascade);
            // Doctor → Department (M:1)
            builder.Entity<Doctor>()
               .HasOne(d => d.Department)
               .WithMany(dep => dep.doctors)
               .HasForeignKey(d => d.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
          
            // Receptionist → ApplicationUser (1:1)
            builder.Entity<resptionist>().
             HasOne(p => p.user).
             WithOne().
             HasForeignKey<resptionist>(p => p.userid).
             OnDelete(DeleteBehavior.Cascade);
            //appoinment --> doctor (m:1)
            builder.Entity<Appoinment>().
                HasOne(a=>a.Doctor).
                WithMany(d=>d.appoinments).
                HasForeignKey(a=>a.doctorid).
                OnDelete(DeleteBehavior.Restrict);
            // Appointment → Patient (M:1)
            builder.Entity<Appoinment>().
                HasOne(a=>a.Patient).
                WithMany(d=>d.appoinments).
                HasForeignKey(a=>a.patientid).
                OnDelete(DeleteBehavior.Restrict);
            // Appointment → Receptionist (M:1)
            builder.Entity<Appoinment>().
             HasOne(a => a.resptionist).
             WithMany(d => d.appoinments).
             HasForeignKey(a => a.resptionistidid).
             OnDelete(DeleteBehavior.Restrict);
            // Appointment → Visit (1:1 optional)
            builder.Entity<Visit>().
                HasOne(v => v.Appoinment).
                WithOne(a => a.Visit).
                HasForeignKey<Visit>(v => v.appoinmentid);

            // Visit → Prescription (1:1)
            // =========================
            builder.Entity<Visit>()
                .HasOne(v => v.Prescription)
                .WithOne(p => p.Visit)
                .HasForeignKey<Prescription>(p => p.visitid).OnDelete(DeleteBehavior.Cascade);

            // Visit → Invoice (1:1)
            // =========================
             builder.Entity<Visit>()
                .HasOne(v => v.Invoice)
                .WithOne(i => i.Visit)
                .HasForeignKey<Invoice>(i => i.VisitId).OnDelete(DeleteBehavior.Cascade);

            // Prescription → Items (1:M)
            builder.Entity<Prescriptionitems>().
                HasOne(i=>i.Prescription).
                WithMany(p=>p.items).
                HasForeignKey(i=>i.prescriptionid).
                OnDelete(DeleteBehavior.Cascade);
            // Medicine → PrescriptionItems (1:M)
            builder.Entity<Prescriptionitems>().
             HasOne(i => i.Medicine).
             WithMany(p => p.Prescriptionitems).
             HasForeignKey(i => i.mdeicineid).
             OnDelete(DeleteBehavior.Restrict);
            //department --> applicationuser (1:m)
            builder.Entity<Department>()
            .HasOne(d => d.user)
            .WithMany()
            .HasForeignKey(d => d.userid)
            .OnDelete(DeleteBehavior.Restrict);
            // Doctor → Department (M:1)
            builder.Entity<Doctor>()
               .HasOne(d => d.Department)
               .WithMany(dep => dep.doctors)
               .HasForeignKey(d => d.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
            //------------------------------------------------------------------------------


            //-------------------------------VALIDATION-------------------------------------

            builder.Entity<Department>(entity =>
            {
                // 🔑 Primary Key
                entity.HasKey(d => d.DepartmentId);

                // 📌 Name
                entity.Property(d => d.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                // 📌 FloorNumber
                entity.Property(d => d.FloorNumber)
                      .IsRequired();

                // 📌 Phone
                entity.Property(d => d.Phone)
                      .IsRequired()
                      .HasMaxLength(20);

                // 📌 isactive
                entity.Property(d => d.isactive)
                      .HasDefaultValue(true);

                // 📌 createdat
                entity.Property(d => d.createdat)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()");

                // 📌 updatedat
                entity.Property(d => d.updatedat)
                      .IsRequired(false);

                // 📌 description
                entity.Property(d => d.description)
                      .HasMaxLength(500);
                builder.Entity<Patient>(entity =>
                {
                    entity.HasKey(p => p.patientid);
                    entity.Property(p => p.patientname).IsRequired().HasMaxLength(100);
                    entity.Property(p => p.phonenumber).IsRequired().HasMaxLength(20);
                    entity.Property(p => p.nationalid).HasColumnType("bigint");
                    entity.Property(p => p.isvalid).HasDefaultValue(true);
                });
                builder.Entity<Appoinment>(entity =>
                {
                    entity.HasKey(p => p.appoimentid);
                    entity.Property(p=>p.PhoneNumber).IsRequired().HasMaxLength(20);
                    entity.Property(p => p.Appoinmentdate).IsRequired();
                    entity.Property(p => p.startat).IsRequired();
                    entity.Property(p => p.endat).IsRequired();
                    entity.Property(p => p.updateat).IsRequired();
                    entity.Property(p => p.notes).IsRequired().HasMaxLength(200);
                    entity.Property(e => e.cost)
                          .IsRequired()
                          .HasColumnType("decimal(10,2)");
                     entity.Property(e => e.type)
                          .IsRequired()
                          .HasConversion<int>();
                });
            
            });
        }
        // DbSets
        public DbSet<Category> categories { get; set; } 
        public DbSet<Department> Departments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appoinment> Appointments { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Prescriptionitems> PrescriptionItems { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Admin> Admins { get; set; }
         public DbSet<resptionist> resptionists { get; set; }
    }
}
