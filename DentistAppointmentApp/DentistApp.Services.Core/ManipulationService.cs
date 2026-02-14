namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ManipulationViewModels;
    using static DentistApp.GCommon.ValidationMessages;

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
            if (await IsManipulationNameDuplicatedAsync(manipulationToCreate.Name))
            {
                throw new Exception(DuplicateManipulationNameValidationMessage);
            }
            ManipulationType currentManipulation = new ManipulationType
            {
                Name = manipulationToCreate.Name.TrimEnd(),
                PriceRange = manipulationToCreate.PriceRange

            };
            await dbContext.ManipulationTypes.AddAsync(currentManipulation);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteManipulationAsync(Guid id)
        {
            ManipulationType? manipulationToDelete = await this.GetManipulationByIdAsync(id);

            if (manipulationToDelete==null)
            {
                throw new Exception(ManipulationCannotBeFoundValidationMessage);
            }

            manipulationToDelete.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task EditManipulationAsync(ManipulationEditViewModel manipulationToEdit)
        {
            if (await IsManipulationNameDuplicatedAsync(manipulationToEdit.Name,manipulationToEdit.ManipulationId))
            {
                throw new Exception(DuplicateManipulationNameValidationMessage);
            }
            ManipulationType? editManipulation = await this.GetManipulationByIdAsync(manipulationToEdit.ManipulationId!.Value);
            if (editManipulation == null)
            {
                throw new Exception(ManipulationCannotBeFoundValidationMessage);
            }
            editManipulation.Name=manipulationToEdit.Name;
            editManipulation.PriceRange=manipulationToEdit.PriceRange;
            await dbContext.SaveChangesAsync();
        }

        public async Task <IEnumerable<ManipulationViewAllViewModel>> GetAllManipulationTypesAsync()
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

        public async Task <ManipulationType?> GetManipulationByIdAsync(Guid id)
        {
            return await dbContext
                .ManipulationTypes
                .SingleOrDefaultAsync(m => m.IsDeleted == false 
                && m.ManipulationId == id);
        }

        public async Task <ManipulationEditViewModel> GetManipulationEditViewModelAsync(Guid id)
        {
            ManipulationType? manipulationToEdit = await this.GetManipulationByIdAsync(id);
            if (manipulationToEdit == null)
            {
                throw new Exception(ManipulationCannotBeFoundValidationMessage);
            }
            ManipulationEditViewModel editViewModel = new ManipulationEditViewModel
            {
                ManipulationId = manipulationToEdit.ManipulationId,
                Name = manipulationToEdit.Name,
                PriceRange = manipulationToEdit.PriceRange
            };
            return editViewModel;
        }

        public async Task <IEnumerable<DropDown>> GetManipulationTypesAsync()
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

        public async Task <bool> IsManipulationNameDuplicatedAsync(string name, Guid? id=null)
        {
            return await this.dbContext
                .ManipulationTypes
                .AsNoTracking()
                .AnyAsync(m => m.Name == name 
                && m.IsDeleted == false
                && (id == null || m.ManipulationId != id));
        }

        public async Task <bool> ValidateManipulationTypesAsync(Guid currentManipulation)
        {
            return dbContext.ManipulationTypes
                .Any(m => m.ManipulationId == currentManipulation);
        }
    }
}
