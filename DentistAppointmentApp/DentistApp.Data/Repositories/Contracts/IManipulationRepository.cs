using DentistApp.Data.Models;
using DentistApp.Data.Repositories.Dtos;
using DentistApp.ViewModels.ManipulationViewModels;

namespace DentistApp.Data.Repositories.Contracts
{
    public interface IManipulationRepository
    {
        Task AddAsync(ManipulationType manipulationToAdd);
        Task SaveChangesAsync();

        Task<ManipulationType?> GetManipulationByIdAsync(Guid id);

        Task<IEnumerable<ManipulationListingDto>> GetAllManipulationTypesAsync();

        Task<IEnumerable<ManipulationDropdownListingDto>> GetManipulationTypesAsync();

        Task<bool> IsManipulationNameDuplicatedAsync(string name, Guid? id = null);

        Task<bool> ValidateManipulationTypesAsync(Guid currentManipulation);

    }
}
