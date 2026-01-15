
namespace DentistApp.Data.Configuration
{
    using DentistApp.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> entity)
        {
            entity
                .HasQueryFilter(r => r.IsDeleted == false);

            entity
               .HasOne(p => p.Patient)
               .WithMany(r => r.Reservations)
               .HasForeignKey(p => p.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(p => p.ManipulationType)
                .WithMany(r => r.Reservations)
                .HasForeignKey(p => p.ManipulationTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
