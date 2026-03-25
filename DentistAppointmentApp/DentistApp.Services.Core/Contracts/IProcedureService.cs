namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.ViewModels.ProcedureViewModels;
    
    public interface IProcedureService
    {
        Task <ProcedurePaginationViewModel> GetAllProceduresViewModelsAsync(string userId, string? searchQuery=null, int page = 1);

        Task <ProcedureCreateViewModel> GetCreateViewModelAsync();

        Task CreateProcedureAsync(ProcedureCreateViewModel procedureToCreate, string dentistId);

        Task <Procedure?> GetProcedureByIdAsync(Guid procedureId);

        Task DeleteProcedureByIdAsync(Guid procedureId);

        Task <ProcedureCreateViewModel> LoadProcedureEditViewModelByIdAsync(Guid procedureId);

        Task EditProcedureAsync(ProcedureCreateViewModel procedureToEdit, string dentistId);

        Task <bool> IsProcedureDateInTheFuture(DateTime procedureDate);

        Task <bool> IsProcedureValid(Guid procedureId);

        Task<Guid?> GetLatestProcedureByUserIdAsync(string userId);
    }
}
