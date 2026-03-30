namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using static DentistApp.GCommon.Roles;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using DentistApp.Data.Repositories.Contracts;

    public class PatientService : IPatientService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPatientRepository patientRepository;
        public PatientService(UserManager<ApplicationUser> userManager, IPatientRepository patientRepository)
        {
            this.userManager = userManager;
            this.patientRepository = patientRepository;
        }
        public async Task <IEnumerable<DropDown>> GetPatientsAsync()
        {
            return await patientRepository.GetPatientsAsync();
        }
        public async Task <string?> GetDentistIdAsync()
        {
            IEnumerable<ApplicationUser> dentists = await userManager
                .GetUsersInRoleAsync(DentistRoleName);

            return dentists
                .FirstOrDefault()?.Id;
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
            return await patientRepository.IsUserInDbByIdAsync(userId);
        }
    }
}
