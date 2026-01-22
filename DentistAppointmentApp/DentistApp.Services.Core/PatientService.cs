using DentistApp.Services.Core.Contracts;
using DentistApp.Services.Core.Models;
using DentistAppointmentApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DentistApp.Services.Core
{
    public class PatientService : IPatientService
    {
        
        private readonly DentistAppDbContext dbContext;
        public PatientService(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<LookupItem>> GetPatientsAsync()
        {
            return await dbContext.Users.
                AsNoTracking().
                Where(u => u.IsDeleted == false)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new LookupItem(Guid.Parse(u.Id), $"{u.FirstName} {u.LastName}"))
                .ToArrayAsync();
        }

       
    }
}
