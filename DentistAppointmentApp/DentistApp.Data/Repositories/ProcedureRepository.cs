namespace DentistApp.Data.Repositories
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using Microsoft.EntityFrameworkCore;

    public class ProcedureRepository : IProcedureRepository
    {
        private readonly DentistAppDbContext dbContext;

        public ProcedureRepository(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Procedure procedure)
        {
            await dbContext.Procedures.AddAsync(procedure);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task SoftDeleteAppointmentAsync(Procedure procedure)
        {
            procedure.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task<IQueryable<Procedure>> GetQueryableProceduresAsync(string userId)
        {
            return dbContext
               .Procedures
               .AsNoTracking()
               .Include(p => p.ManipulationType)
               .Include(p => p.Dentist)
               .Include(p => p.Patient)
               .Where(p => p.DentistId == userId || p.PatientId == userId);
        }

        public async Task<Procedure?> GetProcedureByIdAsync(Guid procedureId)
        {
            return await dbContext
                .Procedures
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.ProcedureId == procedureId);
        }

        public async Task<bool> IsProcedureValidAsync(Guid procedureId)
        {
            return await dbContext
                .Procedures
                .AnyAsync(a => a.IsDeleted == false &&
                a.ProcedureId == procedureId);
        }

        public async Task<Guid?> GetLatestProcedureByUserIdAsync(string userId)
        {
            return await dbContext.Procedures
            .AsNoTracking()
            .Where(p => p.PatientId == userId)
            .OrderByDescending(p => p.Date)
            .Select(p => p.ProcedureId)
            .FirstOrDefaultAsync();
        }
    }
}
