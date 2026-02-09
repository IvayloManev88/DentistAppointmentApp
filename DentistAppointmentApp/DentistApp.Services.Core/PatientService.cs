namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using static DentistApp.GCommon.Roles;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;    

    public class PatientService : IPatientService
    {
        private readonly DentistAppDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        public PatientService(DentistAppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }
        public async Task <IEnumerable<DropDown>> GetPatientsAsync()
        {
            return await dbContext.Users.
                AsNoTracking().
                Where(u => u.IsDeleted == false)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new DropDown
                {
                    Id=Guid.Parse(u.Id), 
                    Name=$"{u.FirstName} {u.LastName}"
                })
                .ToArrayAsync();
        }
        public async Task <string?> GetDentistIdAsync()
        {
            IEnumerable<ApplicationUser> dentists = await userManager
                .GetUsersInRoleAsync(DentistRoleName);

            return dentists
                .FirstOrDefault()?.Id.ToString();
        }

        public async Task <bool> IsUserDentistByIdAsync(string userId)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            return await userManager.IsInRoleAsync(user, DentistRoleName);
        }

        public async Task <bool> IsUserInDbByIdAsync(string userId)
        {
            return dbContext.Users
                .Any(u => u.Id == userId);
        }
    }
}
