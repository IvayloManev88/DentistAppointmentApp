namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.ViewModels.ProcedureViewModels;
    public interface IProcedureService
    {
        Task<IEnumerable<ProcedureViewViewModel>> GetAllProceduresViewModelsAsync(string userId);

        Task<ProcedureCreateViewModel> GetCreateViewModelAsync();

        Task CreateProcedureAsync(ProcedureCreateViewModel procedureToCreate, string dentistId);

        Task <Procedure?> GetProcedureByIdAsync(Guid procedureId);

        Task DeleteProcedureByIdAsync(Guid procedureId);

        Task <ProcedureCreateViewModel> LoadProcedureEditViewModelByIdAsync(Guid procedureId);

        Task EditProcedureAsync(ProcedureCreateViewModel procedureToEdit, Procedure editProcedure, string dentistId);
    }
}
