using DentistApp.Data.Models;

namespace DentistApp.Data.Repositories.Contracts
{
    public interface IProcedureRepository
    {
        Task AddAsync(Procedure procedure);

        Task SaveChangesAsync();
        Task SoftDeleteAppointmentAsync(Procedure procedure);

        Task<IQueryable<Procedure>> GetQueryableProceduresAsync(string userId);

        Task<Procedure?> GetProcedureByIdAsync(Guid procedureId);

        Task<bool> IsProcedureValidAsync(Guid procedureId);

        Task<Guid?> GetLatestProcedureByUserIdAsync(string userId);
    }
}
