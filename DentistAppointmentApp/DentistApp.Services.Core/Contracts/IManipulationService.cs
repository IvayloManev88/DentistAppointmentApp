using DentistApp.Services.Core.Models;

namespace DentistApp.Services.Core.Contracts
{
   
    public interface IManipulationService
    {
        
        Task<IEnumerable<LookupItem>> GetManipulationTypesAsync();
    }
}
