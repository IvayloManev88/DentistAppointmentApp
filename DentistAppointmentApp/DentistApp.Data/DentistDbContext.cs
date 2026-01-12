namespace DentistAppointmentApp.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DentistDbContext(DbContextOptions<DentistDbContext> options) : IdentityDbContext(options)
    {
    }
}
