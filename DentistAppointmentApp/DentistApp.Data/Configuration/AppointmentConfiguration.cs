
namespace DentistApp.Data.Configuration
{
    using DentistApp.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> entity)
        {
            entity
                .HasQueryFilter(r => r.IsDeleted == false);

            entity
               .HasOne(p => p.Patient)
               .WithMany(r => r.PatientAppointments)
               .HasForeignKey(p => p.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

            entity
              .HasOne(p => p.Dentist)
              .WithMany(r => r.DentistAppointments)
              .HasForeignKey(p => p.DentistId)
              .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(p => p.ManipulationType)
                .WithMany(r => r.Appointments)
                .HasForeignKey(p => p.ManipulationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);
            entity
                .Property(p => p.IsConfirmed)
                .HasDefaultValue(true);
        }
    }
}
