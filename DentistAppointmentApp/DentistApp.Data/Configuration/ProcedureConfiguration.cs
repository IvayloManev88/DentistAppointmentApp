using DentistApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DentistApp.Data.Configuration
{
    public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
    {
        public void Configure(EntityTypeBuilder<Procedure> entity)
        {
            entity
                .HasQueryFilter(p => p.IsDeleted == false);

            entity
                .HasOne(p => p.Patient)
                .WithMany(pat => pat.PatientProcedures)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
             .HasOne(p => p.Dentist)
             .WithMany(den => den.DentistProcedures)
             .HasForeignKey(p => p.DentistId)
             .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(p => p.ManipulationType)
                .WithMany(proc => proc.Procedures)
                .HasForeignKey(p => p.ManipulationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);
        }
    }

}

