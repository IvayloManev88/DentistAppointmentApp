namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.ViewModels;
    public interface IPatientService
    {
        Task<IEnumerable<DropDown>> GetPatientsAsync();

        Task<string?> GetDentistIdAsync();

        Task<bool> IsUserInDbByIdAsync(string userId);
    }
}
