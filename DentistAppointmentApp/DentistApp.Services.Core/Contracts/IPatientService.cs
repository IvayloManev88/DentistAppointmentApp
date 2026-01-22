using DentistApp.Services.Core.Models;


namespace DentistApp.Services.Core.Contracts
{
    public interface IPatientService
    {
        Task<IEnumerable<LookupItem>> GetPatientsAsync();
    }
}
