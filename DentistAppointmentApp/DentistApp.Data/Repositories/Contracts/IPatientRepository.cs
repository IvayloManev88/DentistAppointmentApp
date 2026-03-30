namespace DentistApp.Data.Repositories.Contracts
{
    using DentistApp.ViewModels;
    public interface IPatientRepository
    {
        Task<IEnumerable<DropDown>> GetPatientsAsync();

        Task<bool> IsUserInDbByIdAsync(string userId);
    }
}
