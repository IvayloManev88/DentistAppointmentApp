using DentistApp.Data.Repositories.Contracts;
using DentistApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DentistApp.Data.Repositories
{
    public class PatientRepository:IPatientRepository
    {
        private readonly DentistAppDbContext dbContext;

        public PatientRepository(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<DropDown>> GetPatientsAsync()
        {
            return await dbContext.Users.
                AsNoTracking().
                Where(u => u.IsDeleted == false)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new DropDown
                {
                    Id = Guid.Parse(u.Id),
                    Name = $"{u.FirstName} {u.LastName}"
                })
                .ToArrayAsync();
        }

        public async Task<bool> IsUserInDbByIdAsync(string userId)
        {
            return dbContext.Users
                .Any(u => u.Id == userId);
        }
    }
}
