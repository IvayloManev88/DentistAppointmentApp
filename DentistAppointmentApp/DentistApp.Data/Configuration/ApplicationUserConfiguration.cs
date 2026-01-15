namespace DentistApp.Data.Configuration
{
    using DentistApp.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {

        public void Configure(EntityTypeBuilder<ApplicationUser> entity)
        {
            entity
                 .HasQueryFilter(u => u.IsDeleted == false);
        }
    }
    
    
}
