namespace DentistApp.Services.UnitTest
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos.ManipulationDtos;
    using DentistApp.Services.Core;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ManipulationViewModels;
    using NSubstitute;
    using static DentistApp.GCommon.ValidationMessages;

    public class ManipulationServiceTests
    {
        private readonly IManipulationRepository manipulationRepository;

        private readonly ManipulationService manipulationService;

        public ManipulationServiceTests()
        {
            manipulationRepository = Substitute.For<IManipulationRepository>();

            manipulationService = new ManipulationService(manipulationRepository);
        }

        [Fact]
        public async Task CreateManipulationAsync_ShouldThrow_WhenManipulationNameAlreadyExists()
        {
            // Arrange
            ManipulationCreateViewModel testModel = new ManipulationCreateViewModel
            {
                Name = "Cleaning",
                PriceRange = "50-100"
            };

            manipulationRepository
                .IsManipulationNameDuplicatedAsync(testModel.Name)
                .Returns(true);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(async () =>
                await manipulationService
                .CreateManipulationAsync(testModel));

            Assert.Equal(DuplicateManipulationNameValidationMessage, ex.Message);
        }

        [Fact]
        public async Task CreateManipulationAsync_ShouldAddManipulationAndSave_WhenNameIsNotDuplicated()
        {
            // Arrange
            ManipulationCreateViewModel testModel = new ManipulationCreateViewModel
            {
                Name = "Cleaning",
                PriceRange = "50-100"
            };

            manipulationRepository
                .IsManipulationNameDuplicatedAsync(testModel.Name)
                .Returns(false);

            // Act
            await manipulationService.CreateManipulationAsync(testModel);

            // Assert
            await manipulationRepository
                .Received(1)
                .AddAsync(Arg.Is<ManipulationType>(m =>
                    m.Name == "Cleaning" &&
                    m.PriceRange == "50-100"));

            await manipulationRepository
                .Received(1)
                .SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteManipulationAsync_ShouldThrow_WhenManipulationIsNull()
        {
            // Arrange
            Guid manipulationGuid = Guid.NewGuid();
            manipulationRepository
                .GetManipulationByIdAsync(manipulationGuid)
                .Returns((ManipulationType?)null);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(async () =>
                await manipulationService
                .DeleteManipulationAsync(manipulationGuid));

            Assert.Equal(ManipulationCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task DeleteManipulationAsync_ShouldDelete_WhenManipulationIsNotNull()
        {
            // Arrange
            Guid manipulationGuid = Guid.NewGuid();
            ManipulationType testModel = new ManipulationType
            {
                ManipulationId = manipulationGuid,
                Name = "Cleaning",
                PriceRange = "50-100"
            };
            manipulationRepository
                .GetManipulationByIdAsync(manipulationGuid)
                .Returns(testModel);

            // Act 
            await manipulationService
                .DeleteManipulationAsync(manipulationGuid);

            //Assert
            Assert.True(testModel.IsDeleted);

            await manipulationRepository
                .Received(1)
                .SaveChangesAsync();
        }

        [Fact]
        public async Task EditManipulationAsync_ShouldThrow_WhenManipulationNameIsDuplicated()
        {
            // Arrange
            Guid manipulationGuid = Guid.NewGuid();

            ManipulationEditViewModel testModel = new ManipulationEditViewModel
            {
                ManipulationId = manipulationGuid,
                Name = "Cleaning",
                PriceRange = "50-100"
            };

            manipulationRepository
                .IsManipulationNameDuplicatedAsync(testModel.Name, testModel.ManipulationId)
                .Returns(true);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(async () =>
                await manipulationService.EditManipulationAsync(testModel));

            Assert.Equal(DuplicateManipulationNameValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditManipulationAsync_ShouldThrow_WhenManipulationCannotBeFound()
        {
            // Arrange
            Guid manipulationGuid = Guid.NewGuid();

            ManipulationEditViewModel testModel = new ManipulationEditViewModel
            {
                ManipulationId = manipulationGuid,
                Name = "Cleaning",
                PriceRange = "50-100"
            };

            manipulationRepository
                .IsManipulationNameDuplicatedAsync(testModel.Name, testModel.ManipulationId)
                .Returns(false);

            manipulationRepository
                .GetManipulationByIdAsync(manipulationGuid)
                .Returns(Task.FromResult<ManipulationType?>(null));

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(async () =>
                await manipulationService.EditManipulationAsync(testModel));

            Assert.Equal(ManipulationCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditManipulationAsync_ShouldEditManipulationAndSaveChanges_WhenInputIsValid()
        {
            // Arrange
            Guid manipulationGuid = Guid.NewGuid();

            ManipulationEditViewModel testModel = new ManipulationEditViewModel
            {
                ManipulationId = manipulationGuid,
                Name = "Updated Cleaning",
                PriceRange = "80-120"
            };

            ManipulationType manipulationEntity = new ManipulationType
            {
                ManipulationId = manipulationGuid,
                Name = "Old Cleaning",
                PriceRange = "50-100"
            };

            manipulationRepository
                .IsManipulationNameDuplicatedAsync(testModel.Name, testModel.ManipulationId)
                .Returns(false);

            manipulationRepository
                .GetManipulationByIdAsync(manipulationGuid)
                .Returns(manipulationEntity);

            // Act
            await manipulationService.EditManipulationAsync(testModel);

            // Assert
            Assert.Equal("Updated Cleaning", manipulationEntity.Name);
            Assert.Equal("80-120", manipulationEntity.PriceRange);
        }

        [Fact]
        public async Task GetAllManipulationTypesAsync_ShouldReturnMappedManipulations()
        {
            // Arrange
            IEnumerable<ManipulationListingDto> repositoryData = new List<ManipulationListingDto>
            {
                  new ManipulationListingDto
                  {
                  ManipulationId = Guid.NewGuid().ToString(),
                  Name = "Cleaning",
                  PriceRange = "50-100"
                  }
            };

            manipulationRepository
                .GetAllManipulationTypesAsync()
                .Returns(repositoryData);

            // Act
            IEnumerable<ManipulationViewAllViewModel> result =
                await manipulationService.GetAllManipulationTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            ManipulationViewAllViewModel testModel = result.First();
            Assert.Equal(repositoryData.First().ManipulationId, testModel.ManipulationId);
            Assert.Equal("Cleaning", testModel.Name);
            Assert.Equal("50-100", testModel.PriceRange);
        }

        [Fact]
        public async Task GetManipulationEditViewModelAsync_ShouldThrow_WhenManipulationIsNull()
        {
            // Arrange
            Guid manipulationGuid = Guid.NewGuid();

            manipulationRepository
                .GetManipulationByIdAsync(manipulationGuid)
                .Returns(Task.FromResult<ManipulationType?>(null));

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(async () =>
                await manipulationService.GetManipulationEditViewModelAsync(manipulationGuid));

            Assert.Equal(ManipulationCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task GetManipulationEditViewModelAsync_ShouldReturnMappedViewModel_WhenManipulationExists()
        {
            // Arrange
            Guid manipulationGuid = Guid.NewGuid();

            ManipulationType testModel = new ManipulationType
            {
                ManipulationId = manipulationGuid,
                Name = "Cleaning",
                PriceRange = "50-100"
            };

            manipulationRepository
                .GetManipulationByIdAsync(manipulationGuid)
                .Returns(testModel);

            // Act
            ManipulationEditViewModel result =
                await manipulationService.GetManipulationEditViewModelAsync(manipulationGuid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(manipulationGuid, result.ManipulationId);
            Assert.Equal("Cleaning", result.Name);
            Assert.Equal("50-100", result.PriceRange);
        }

        [Fact]
        public async Task GetManipulationTypesAsync_ShouldReturnMappedDropDowns()
        {
            // Arrange
            IEnumerable<ManipulationDropdownListingDto> repositoryData = new List<ManipulationDropdownListingDto>
            {
                  new ManipulationDropdownListingDto
                  {
                  ManipulationId = Guid.NewGuid(),
                  ManipulationName = "Cleaning"
                  }
            };

            manipulationRepository
                .GetManipulationTypesAsync()
                .Returns(repositoryData);

            // Act
            IEnumerable<DropDown> result = await manipulationService.GetManipulationTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            DropDown testDropDown = result.First();
            Assert.Equal(repositoryData.First().ManipulationId, testDropDown.Id);
            Assert.Equal("Cleaning", testDropDown.Name);
        }

        [Fact]
        public async Task GetManipulationTypesAsync_ShouldReturnEmptyCollection_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            manipulationRepository
                .GetManipulationTypesAsync()
                .Returns(Enumerable.Empty<ManipulationDropdownListingDto>());

            // Act
            IEnumerable<DropDown> result = await manipulationService.GetManipulationTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
