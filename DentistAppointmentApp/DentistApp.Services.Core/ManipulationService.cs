namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ManipulationViewModels;
    using static DentistApp.GCommon.ValidationMessages;

    using Microsoft.EntityFrameworkCore;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos.ManipulationDtos;

    public class ManipulationService : IManipulationService
    {
        private readonly IManipulationRepository manipulationRepository;
        public ManipulationService(IManipulationRepository manipulationRepository)
        {
            this.manipulationRepository = manipulationRepository;
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
            await manipulationRepository.AddAsync(currentManipulation);
            await manipulationRepository.SaveChangesAsync();
        }

        public async Task DeleteManipulationAsync(Guid id)
        {
            ManipulationType? manipulationToDelete = await manipulationRepository
                .GetManipulationByIdAsync(id);

            if (manipulationToDelete==null)
            {
                throw new Exception(ManipulationCannotBeFoundValidationMessage);
            }

            manipulationToDelete.IsDeleted = true;
            await manipulationRepository.SaveChangesAsync();
        }

        public async Task EditManipulationAsync(ManipulationEditViewModel manipulationToEdit)
        {
            if (await IsManipulationNameDuplicatedAsync(manipulationToEdit.Name,manipulationToEdit.ManipulationId))
            {
                throw new Exception(DuplicateManipulationNameValidationMessage);
            }
            ManipulationType? editManipulation = await manipulationRepository
                .GetManipulationByIdAsync(manipulationToEdit.ManipulationId!.Value);

            if (editManipulation == null)
            {
                throw new Exception(ManipulationCannotBeFoundValidationMessage);
            }

            editManipulation.Name=manipulationToEdit.Name;
            editManipulation.PriceRange=manipulationToEdit.PriceRange;

            await manipulationRepository.SaveChangesAsync();
        }

        public async Task <IEnumerable<ManipulationViewAllViewModel>> GetAllManipulationTypesAsync()
        {
            IEnumerable<ManipulationListingDto> manipulationsToView = await manipulationRepository
                .GetAllManipulationTypesAsync();

            return  manipulationsToView
                    .Select(m => new ManipulationViewAllViewModel
                    {
                        ManipulationId = m.ManipulationId,
                        Name = m.Name,
                        PriceRange = m.PriceRange
                    });
        }
              
        public async Task <ManipulationEditViewModel> GetManipulationEditViewModelAsync(Guid id)
        {
            ManipulationType? manipulationToEdit = await manipulationRepository
                .GetManipulationByIdAsync(id);

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

        public async Task<ManipulationType?> GetManipulationByIdAsync(Guid id)
        {
            return await manipulationRepository
                .GetManipulationByIdAsync(id);
        }

        public async Task <IEnumerable<DropDown>> GetManipulationTypesAsync()
        {
            IEnumerable<ManipulationDropdownListingDto> manipulationsForDropDown = await manipulationRepository
                .GetManipulationTypesAsync();

            return manipulationsForDropDown
                .Select(mt => new DropDown
                {
                    Id = mt.ManipulationId,
                    Name = mt.ManipulationName
                });   
        }

        public async Task <bool> IsManipulationNameDuplicatedAsync(string name, Guid? id=null)
        {
            return await manipulationRepository
                .IsManipulationNameDuplicatedAsync(name, id);
        }

        public async Task <bool> ValidateManipulationTypesAsync(Guid currentManipulation)
        {
            return await manipulationRepository
                .ValidateManipulationTypesAsync(currentManipulation);
        }
    }
}
