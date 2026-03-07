namespace DentistApp.Data.Configuration
{
    using DentistApp.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> entity)
        {
            entity
                .HasQueryFilter(f => f.IsDeleted == false);
            entity
                .Property(f => f.IsDeleted)
                .HasDefaultValue(false);
            entity
                .HasOne(f => f.Procedure)
                .WithOne(p => p.Feedback)
                .HasForeignKey<Feedback>(pr => pr.ProcedureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
