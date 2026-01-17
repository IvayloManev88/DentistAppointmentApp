using DentistApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DentistApp.Data.Configuration
{
    public class ManipulationTypeConfiguration : IEntityTypeConfiguration<ManipulationType>
    {
        public void Configure(EntityTypeBuilder<ManipulationType> entity)
        {
            entity
                .HasQueryFilter(u => u.IsDeleted == false);
            entity
                .Property(p => p.IsDeleted)
                .HasDefaultValue(false);

        }
    }
}
