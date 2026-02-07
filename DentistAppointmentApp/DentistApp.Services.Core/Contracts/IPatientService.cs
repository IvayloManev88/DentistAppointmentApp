namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.ViewModels;
    public interface IPatientService
    {
        Task<IEnumerable<DropDown>> GetPatientsAsync();

        Task<string?> GetDentistIdAsync();
    }
}
