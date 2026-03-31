namespace DentistApp.Data.Repositories
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos.ManipulationDtos;
    using Microsoft.EntityFrameworkCore;

    public class ManipulationRepository:IManipulationRepository
    {
        private readonly DentistAppDbContext dbContext;
        public ManipulationRepository(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(ManipulationType manipulationToAdd)
        {
            await dbContext.ManipulationTypes
                .AddAsync(manipulationToAdd);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task<ManipulationType?> GetManipulationByIdAsync(Guid id)
        {
            return await dbContext
                .ManipulationTypes
                .SingleOrDefaultAsync(m => m.IsDeleted == false
                && m.ManipulationId == id);
        }

        public async Task<IEnumerable<ManipulationListingDto>> GetAllManipulationTypesAsync()
        {
            return await dbContext
                .ManipulationTypes
                    .AsNoTracking()
                    .Where(m => m.IsDeleted == false)
                    .OrderBy(m => m.Name)
                    .Select(m => new ManipulationListingDto
                    {
                        ManipulationId = m.ManipulationId.ToString(),
                        Name = m.Name,
                        PriceRange = m.PriceRange
                    }).ToArrayAsync();
        }

        public async Task<IEnumerable<ManipulationDropdownListingDto>> GetManipulationTypesAsync()
        {
            return await dbContext
                .ManipulationTypes
                    .AsNoTracking()
                    .Where(m => m.IsDeleted == false)
                    .OrderBy(m => m.Name)
                    .Select(m => new ManipulationDropdownListingDto
                    {
                        ManipulationId = m.ManipulationId,
                        ManipulationName = m.Name,
                    }).ToArrayAsync();
        }

        public async Task<bool> IsManipulationNameDuplicatedAsync(string name, Guid? id = null)
        {
            return await this.dbContext
                .ManipulationTypes
                .AsNoTracking()
                .AnyAsync(m => m.Name == name
                && m.IsDeleted == false
                && (id == null || m.ManipulationId != id));
        }

        public async Task<bool> ValidateManipulationTypesAsync(Guid currentManipulation)
        {
            return dbContext.ManipulationTypes
                .Any(m => m.ManipulationId == currentManipulation);
        }
    }
}