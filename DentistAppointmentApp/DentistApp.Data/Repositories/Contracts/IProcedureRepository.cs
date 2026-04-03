namespace DentistApp.Data.Repositories.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Dtos.ProcedureDtos;
    public interface IProcedureRepository
    {
        Task AddAsync(Procedure procedure);

        Task SaveChangesAsync();
        Task SoftDeleteProcedureAsync(Procedure procedure);
        Task<Procedure?> GetProcedureByIdAsync(Guid procedureId);
        Task<bool> IsProcedureValidAsync(Guid procedureId);
        Task<Guid?> GetLatestProcedureByUserIdAsync(string userId);
        Task<(ProcedureListingDto[] procedures, int totalCount)> GetPagedProceduresAsync(string userId, string? searchQuery, int page, int pageSize);
    }
}
