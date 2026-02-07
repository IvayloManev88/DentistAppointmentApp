namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ManipulationViewModels;
    using Microsoft.EntityFrameworkCore;
    
    public class ManipulationService : IManipulationService
    {
        private readonly DentistAppDbContext dbContext;
        public ManipulationService(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateManipulationAsync(ManipulationCreateViewModel manipulationToCreate)
        {
            ManipulationType currentManipulation = new ManipulationType
            {
                Name = manipulationToCreate.Name.TrimEnd(),
                PriceRange = manipulationToCreate.PriceRange

            };
            await dbContext.ManipulationTypes.AddAsync(currentManipulation);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteManipulationAsync(ManipulationType manipulationToDelete)
        {
            manipulationToDelete.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task EditManipulationAsync(ManipulationEditViewModel manipulationToEdit, ManipulationType editedManipulation)
        {
            editedManipulation.Name=manipulationToEdit.Name;
            editedManipulation.PriceRange=manipulationToEdit.PriceRange;
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ManipulationViewAllViewModel>> GetAllManipulationTypesAsync()
        {
            return await dbContext
                .ManipulationTypes
                    .AsNoTracking()
                    .Where(m => m.IsDeleted == false)
                    .OrderBy(m => m.Name)
                    .Select(m => new ManipulationViewAllViewModel
                    {
                        ManipulationId = m.ManipulationId.ToString(),
                        Name = m.Name,
                        PriceRange = m.PriceRange
                    }).ToArrayAsync();
        }

        public async Task<ManipulationType?> GetManipulationByIdAsync(Guid id)
        {
            return await dbContext
                .ManipulationTypes
                .SingleOrDefaultAsync(m => m.IsDeleted == false 
                && m.ManipulationId == id);
        }

        public async Task<ManipulationEditViewModel> GetManipulationEditViewModelAsync(ManipulationType manipulationToEdit)
        {
            ManipulationEditViewModel editViewModel = new ManipulationEditViewModel
            {
                ManipulationId = manipulationToEdit.ManipulationId,
                Name = manipulationToEdit.Name,
                PriceRange = manipulationToEdit.PriceRange
            };
            return editViewModel;
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

        public async Task<bool> IsManipulationNameDuplicatedAsync(string name, Guid? id=null)
        {
            return await this.dbContext
                .ManipulationTypes
                .AsNoTracking()
                .AnyAsync(m => m.Name == name 
                && m.IsDeleted == false
                && (id == null || m.ManipulationId != id));
        }

    }
}
