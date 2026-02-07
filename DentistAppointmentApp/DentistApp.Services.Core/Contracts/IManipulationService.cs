namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.ViewModels;
    public interface IManipulationService
    {
        
        Task<IEnumerable<DropDown>> GetManipulationTypesAsync();
    }
}
