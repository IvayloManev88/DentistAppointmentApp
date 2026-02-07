namespace DentistApp.Services.Core
{
    using DentistApp.Data;

    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;

    using Microsoft.EntityFrameworkCore;
    
    public class ManipulationService : IManipulationService
    {
               private readonly DentistAppDbContext dbContext;
        public ManipulationService(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<DropDown>> GetManipulationTypesAsync()
        {
            return await dbContext.ManipulationTypes
                .Where(mt => mt.IsDeleted == false)
                .OrderBy(mt => mt.Name)
                .Select(mt => new DropDown
                { 
                    Id =mt.ManipulationId,
                    Name = mt.Name 
                })
                .ToArrayAsync();
        }
    }
}
