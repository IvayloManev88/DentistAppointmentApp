

namespace DentistApp.Services.Core
{
    using DentistApp.Services.Core.Contracts;
    using DentistApp.Services.Core.Models;
    using DentistAppointmentApp.Data;
    using Microsoft.EntityFrameworkCore;
    
    public class ManipulationService : IManipulationService
    {
       
        private readonly DentistAppDbContext dbContext;
        public ManipulationService(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<LookupItem>> GetManipulationTypesAsync()
        {
            return await dbContext.ManipulationTypes
                .Where(mt => mt.IsDeleted == false)
                .OrderBy(mt => mt.Name)
                .Select(mt => new LookupItem(mt.ManipulationId, mt.Name))
                .ToArrayAsync();
        }
    }
}
