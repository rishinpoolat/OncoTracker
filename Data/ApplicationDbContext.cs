using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OncoTrack.Models;

namespace OncoTrack.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<TreatmentUpdate> TreatmentUpdates { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure primary keys for string IDs
            builder.Entity<Patient>()
                .HasKey(p => p.PatientId);

            builder.Entity<Doctor>()
                .HasKey(d => d.DoctorId);

            builder.Entity<TreatmentUpdate>()
                .HasKey(t => t.TreatmentUpdateId);

            builder.Entity<Medication>()
                .HasKey(m => m.MedicationId);

            builder.Entity<Appointment>()
                .HasKey(a => a.AppointmentId);

            // Configure Patient-Doctor relationship
            builder.Entity<Patient>()
                .HasOne(p => p.AssignedDoctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(p => p.AssignedDoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Patient-User relationship
            builder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Doctor-User relationship
            builder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure other relationships
            builder.Entity<TreatmentUpdate>()
                .HasOne(t => t.Patient)
                .WithMany(p => p.TreatmentUpdates)
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Medication>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.Medications)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}