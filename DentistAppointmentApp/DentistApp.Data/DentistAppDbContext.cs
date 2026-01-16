namespace DentistAppointmentApp.Data
{
    using DentistApp.Data.Configuration;
    using DentistApp.Data.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    public class DentistAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DentistAppDbContext(DbContextOptions<DentistAppDbContext> options)
        : base(options)
        {

        }
        public virtual DbSet<ManipulationType> ManipulationTypes { get; set; } = null!;
        public virtual DbSet<Procedure> Procedures { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ProcedureConfiguration).Assembly);
        }
    }
}
