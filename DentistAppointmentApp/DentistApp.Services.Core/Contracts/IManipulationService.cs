namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ManipulationViewModels;

    public interface IManipulationService
    {
        
        Task<IEnumerable<DropDown>> GetManipulationTypesAsync();

        Task<IEnumerable<ManipulationViewAllViewModel>> GetAllManipulationTypesAsync();

        Task<bool> IsManipulationNameDuplicatedAsync(string name, Guid? id=null);

        Task CreateManipulationAsync(ManipulationCreateViewModel manipulationToCreate);

        Task <ManipulationType?> GetManipulationByIdAsync(Guid id);

        Task DeleteManipulationAsync(ManipulationType manipulationToDelete);

        Task<ManipulationEditViewModel> GetManipulationEditViewModelAsync(ManipulationType manipulationToEdit);

        Task EditManipulationAsync(ManipulationEditViewModel manipulationToEdit, ManipulationType editedManipulation);

        Task<bool> ValidateManipulationTypesAsync(Guid currentManipulation);
    }
}
