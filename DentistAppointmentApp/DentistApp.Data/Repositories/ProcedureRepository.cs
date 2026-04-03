namespace DentistApp.Data.Repositories
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos.ProcedureDtos;
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

        public async Task SoftDeleteProcedureAsync(Procedure procedure)
        {
            procedure.IsDeleted = true;
            await dbContext.SaveChangesAsync();
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

        public async Task<(ProcedureListingDto[] procedures, int totalCount)> GetPagedProceduresAsync(string userId, string? searchQuery, int page, int pageSize)
        {
            IQueryable<Procedure> query = dbContext.Procedures
                .AsNoTracking()
                .Where(p => p.PatientId == userId || p.DentistId == userId);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string normalizedQuery = searchQuery.ToLower().Trim();

                query = query.Where(p =>
                    (p.Patient.FirstName + " " + p.Patient.LastName).ToLower().Contains(normalizedQuery) ||
                    p.ManipulationType.Name.ToLower().Contains(normalizedQuery));
            }

            int totalCount = await query.CountAsync();

            ProcedureListingDto[] procedures = await query
                .OrderByDescending(p => p.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProcedureListingDto
                {
                    ProcedureId = p.ProcedureId,
                    PatientFirstName = p.Patient.FirstName,
                    PatientLastName = p.Patient.LastName,
                    DentistFirstName = p.Dentist.FirstName,
                    DentistLastName = p.Dentist.LastName,
                    ProcedureDate = p.Date,
                    PatientPhoneNumber = p.PatientPhoneNumber,
                    ManipulationName = p.ManipulationType.Name,
                    DentistNote = p.Note
                })
                .ToArrayAsync();

            return (procedures, totalCount);
        }
    }
}
