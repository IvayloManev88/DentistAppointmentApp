namespace DentistApp.Services.UnitTest
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos.ProcedureDtos;
    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ProcedureViewModels;
    using NSubstitute;
    using System.Globalization;
    using Xunit;

    using static DentistApp.GCommon.GlobalCommon;
    using static DentistApp.GCommon.ValidationMessages;
    public class ProcedureServiceTests
    {
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        private readonly IProcedureRepository procedureRepository;
        private readonly IDateTimeService dateTimeService;

        private readonly ProcedureService procedureService;
        public ProcedureServiceTests()
        {
            manipulationService = Substitute.For<IManipulationService>();
            patientService = Substitute.For<IPatientService>();
            dateTimeService = Substitute.For<IDateTimeService>();
            procedureRepository = Substitute.For<IProcedureRepository>();

            procedureService = new ProcedureService(manipulationService, patientService, procedureRepository, dateTimeService);
        }

        [Fact]
        public async Task CreateProcedureAsync_ShouldThrow_WhenManipulationIsInvalid()
        {
            // Arrange
            ProcedureCreateViewModel model = this.CreateTestProcedureCreateViewModel();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(false);

            dateTimeService.Today()
                .Returns(new DateTime(2026, 01, 01));

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.CreateProcedureAsync(model, dentistId));

            Assert.Equal(ManipulationNotCorrectValidationMessage, ex.Message);
        }

        [Fact]
        public async Task CreateProcedureAsync_ShouldThrow_WhenProcedureDateIsInTheFuture()
        {
            // Arrange
            ProcedureCreateViewModel model = this.CreateTestProcedureCreateViewModel();

            dateTimeService.Today()
                .Returns(new DateTime(2025, 01, 01));


            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.CreateProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureCannotBeInTheFutureValidationMessage, ex.Message);
        }

        [Fact]
        public async Task CreateProcedureAsync_ShouldThrow_WhenCreatorIsNotDentist()
        {
            // Arrange
            ProcedureCreateViewModel model = this.CreateTestProcedureCreateViewModel();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService.Today()
                .Returns(new DateTime(2026, 01, 01));

            patientService
                .IsUserDentistByIdAsync(dentistId)
                .Returns(false);


            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.CreateProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureCreatorIsNotDentistValidationMessage, ex.Message);
        }

        [Fact]
        public async Task CreateProcedureAsync_ShouldThrow_WhenPatientIsNotInDatabase()
        {
            // Arrange
            ProcedureCreateViewModel model = this.CreateTestProcedureCreateViewModel();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService.Today()
                .Returns(new DateTime(2026, 01, 01));

            patientService
                .IsUserDentistByIdAsync(dentistId)
                .Returns(true);

            patientService
                .IsUserInDbByIdAsync(model.PatientId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.CreateProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureCreatorNotInDatabaseValidationMessage, ex.Message);
        }

        [Fact]
        public async Task CreateProcedureAsync_ShouldAddProcedureAndSaveChanges_WhenInputIsValid()
        {
            // Arrange
            ProcedureCreateViewModel model = this.CreateTestProcedureCreateViewModel();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService.Today()
                .Returns(new DateTime(2026, 01, 01));

            patientService
                .IsUserDentistByIdAsync(dentistId)
                .Returns(true);

            patientService
                .IsUserInDbByIdAsync(model.PatientId)
                .Returns(true);

            // Act
            await procedureService.CreateProcedureAsync(model, dentistId);

            // Assert
            await procedureRepository.Received(1).AddAsync(
                Arg.Is<Procedure>(p =>
                    p.PatientId == model.PatientId &&
                    p.DentistId == dentistId &&
                    p.Date == model.ProcedureDate &&
                    p.PatientPhoneNumber == model.PatientPhoneNumber &&
                    p.ManipulationTypeId == model.ManipulationTypeId &&
                    p.Note == model.DentistNote));

            await procedureRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteProcedureByIdAsync_ShouldThrow_WhenProcedureDoesNotExist()
        {
            // Arrange
            Guid procedureId = Guid.NewGuid();

            procedureRepository
                .GetProcedureByIdAsync(procedureId)
                .Returns((Procedure?)null);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.DeleteProcedureByIdAsync(procedureId));

            Assert.Equal(ProcedureCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task DeleteProcedureByIdAsync_ShouldCallSoftDelete_WhenProcedureExists()
        {
            // Arrange
            Guid procedureId = Guid.NewGuid();

            Procedure procedure = new Procedure
            {
                ProcedureId = procedureId
            };

            procedureRepository
                .GetProcedureByIdAsync(procedureId)
                .Returns(procedure);

            // Act
            await procedureService
                .DeleteProcedureByIdAsync(procedureId);

            // Assert
            await procedureRepository
                .Received(1)
                .SoftDeleteProcedureAsync(procedure);
        }


        [Fact]
        public async Task GetAllProceduresViewModelsAsync_ShouldReturnMappedPaginationViewModel()
        {
            // Arrange
            string userId = "user-id";
            string? searchQuery = "cleaning";
            int page = 2;

            ProcedureListingDto[] proceduresDto =
            [
                new ProcedureListingDto
                {
                ProcedureId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                PatientFirstName = "Ivan",
                PatientLastName = "Petrov",
                DentistFirstName = "Dr",
                DentistLastName = "Ivanov",
                ProcedureDate = new DateTime(2025, 1, 10),
                PatientPhoneNumber = "0899999999",
                ManipulationName = "Cleaning",
                DentistNote = "First note"
                }
            ];

            int totalCount = 12;

            procedureRepository
                .GetPagedProceduresAsync(userId, searchQuery, page, 10)
                .Returns((proceduresDto, totalCount));

            // Act
            ProcedurePaginationViewModel result =
                await procedureService.GetAllProceduresViewModelsAsync(userId, searchQuery, page);

            // Assert
            await procedureRepository.Received(1)
                .GetPagedProceduresAsync(userId, searchQuery, page, 10);

            Assert.Equal(searchQuery, result.SearchQuery);
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(10, result.ProceduresPerPage);
            Assert.Equal(totalCount, result.TotalItemsCount);

            ProcedureViewViewModel[] procedures = result.Procedures.ToArray();

            Assert.Single(procedures);

            Assert.Equal("11111111-1111-1111-1111-111111111111", procedures[0].ProcedureId);
            Assert.Equal("Ivan Petrov", procedures[0].PatientProcedureName);
            Assert.Equal("Dr Ivanov", procedures[0].DentistProcedureName);
            Assert.Equal(
                new DateTime(2025, 1, 10).ToString(DateFormat, CultureInfo.InvariantCulture),
                procedures[0].ProcedureDate);
            Assert.Equal("0899999999", procedures[0].PatientProcedurePhoneNumber);
            Assert.Equal("Cleaning", procedures[0].ManipulationName);
            Assert.Equal("First note", procedures[0].ProcedureDentistNote);
        }

        [Fact]
        public async Task GetAllProceduresViewModelsAsync_ShouldReturnEmptyProcedures_WhenRepositoryReturnsNoItems()
        {
            // Arrange
            string userId = "user-id";
            string? searchQuery = "missing";
            int page = 1;

            ProcedureListingDto[] proceduresDto = [];
            int totalCount = 0;

            procedureRepository
                .GetPagedProceduresAsync(userId, searchQuery, page, 10)
                .Returns((proceduresDto, totalCount));

            // Act
            ProcedurePaginationViewModel result =
                await procedureService.GetAllProceduresViewModelsAsync(userId, searchQuery, page);

            // Assert
            await procedureRepository.Received(1)
                .GetPagedProceduresAsync(userId, searchQuery, page, 10);

            Assert.Empty(result.Procedures);
            Assert.Equal(searchQuery, result.SearchQuery);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(10, result.ProceduresPerPage);
            Assert.Equal(0, result.TotalItemsCount);
        }

        [Fact]
        public async Task GetCreateViewModelAsync_ShouldReturnPopulatedViewModel()
        {
            // Arrange
            DateTime fixedDate = new DateTime(2025, 1, 1);

            var manipulationTypes = new List<DropDown>
            {
                new DropDown 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Cleaning" 
                }
            };
            var patients = new List<DropDown>
            {
                new DropDown
                {
                    Id = Guid.NewGuid(),
                    Name = "Ivan Petrov"
                }
            };

            dateTimeService
                .Today()
                .Returns(fixedDate);

            manipulationService
                .GetManipulationTypesAsync()
                .Returns(manipulationTypes);

            patientService
                .GetPatientsAsync()
                .Returns(patients);

            // Act
            ProcedureCreateViewModel result =
                await procedureService.GetCreateViewModelAsync();

            // Assert
            Assert.NotNull(result);

            Assert.Equal(fixedDate, result.ProcedureDate);
            Assert.Equal(manipulationTypes, result.ManipulationTypes);
            Assert.Equal(patients, result.PatientsNames);
        }

        [Fact]
        public async Task LoadProcedureEditViewModelByIdAsync_ShouldThrow_WhenProcedureIsNotFound()
        {
            // Arrange
            Guid procedureId = Guid.NewGuid();

            procedureRepository
                .GetProcedureByIdAsync(procedureId)
                .Returns((Procedure?)null);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.LoadProcedureEditViewModelByIdAsync(procedureId));

            Assert.Equal(ProcedureCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task LoadProcedureEditViewModelByIdAsync_ShouldReturnPopulatedEditViewModel_WhenProcedureExists()
        {
            // Arrange
            Guid procedureId = Guid.NewGuid();

            Procedure procedure = new Procedure
            {
                ProcedureId = procedureId,
                Date = new DateTime(2025, 1, 15),
                PatientId = "patient-id",
                PatientPhoneNumber = "0899999999",
                ManipulationTypeId = Guid.NewGuid(),
                Note = "Test note"
            };

            IEnumerable<DropDown> manipulationTypes = new List<DropDown>
            {
                new DropDown
                {
                    Id = Guid.NewGuid(),
                    Name = "Cleaning"
                },
            };

            IEnumerable<DropDown> patients = new List<DropDown>
            {
                new DropDown
                {
                    Id = Guid.NewGuid(),
                    Name = "Ivan Petrov"
                }
            };

            procedureRepository
                .GetProcedureByIdAsync(procedureId)
                .Returns(procedure);

            manipulationService
                .GetManipulationTypesAsync()
                .Returns(manipulationTypes);

            patientService
                .GetPatientsAsync()
                .Returns(patients);

            // Act
            ProcedureCreateViewModel result =
                await procedureService.LoadProcedureEditViewModelByIdAsync(procedureId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(procedure.ProcedureId, result.ProcedureId);
            Assert.Equal(procedure.Date, result.ProcedureDate);
            Assert.Equal(procedure.PatientId, result.PatientId);
            Assert.Equal(procedure.PatientPhoneNumber, result.PatientPhoneNumber);
            Assert.Equal(procedure.ManipulationTypeId, result.ManipulationTypeId);
            Assert.Equal(procedure.Note, result.DentistNote);
            Assert.Equal(manipulationTypes, result.ManipulationTypes);
            Assert.Equal(patients, result.PatientsNames);
        }

        [Fact]
        public async Task EditProcedureAsync_ShouldThrow_WhenManipulationIsInvalid()
        {
            // Arrange
            var model = CreateTestProcedureCreateViewModel();
            model.ProcedureId = Guid.NewGuid();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.EditProcedureAsync(model, dentistId));

            Assert.Equal(ManipulationNotCorrectValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditProcedureAsync_ShouldThrow_WhenProcedureDateIsInFuture()
        {
            // Arrange
            ProcedureCreateViewModel model = CreateTestProcedureCreateViewModel();
            model.ProcedureId = Guid.NewGuid();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2024, 1, 1));

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.EditProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureCannotBeInTheFutureValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditProcedureAsync_ShouldThrow_WhenDentistIsNotInDatabase()
        {
            // Arrange
            ProcedureCreateViewModel model = CreateTestProcedureCreateViewModel();
            model.ProcedureId = Guid.NewGuid();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 1, 1));

            patientService
                .IsUserInDbByIdAsync(dentistId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.EditProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureDentistNotInDatabaseValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditProcedureAsync_ShouldThrow_WhenPatientIsNotInDatabase()
        {
            // Arrange
            ProcedureCreateViewModel model = CreateTestProcedureCreateViewModel();
            model.ProcedureId = Guid.NewGuid();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 1, 1));

            patientService
                .IsUserInDbByIdAsync(dentistId)
                .Returns(true);

            patientService
                .IsUserInDbByIdAsync(model.PatientId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.EditProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureCreatorNotInDatabaseValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditProcedureAsync_ShouldThrow_WhenDentistUserIsNotDentist()
        {
            // Arrange
            ProcedureCreateViewModel model = CreateTestProcedureCreateViewModel();
            model.ProcedureId = Guid.NewGuid();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 1, 1));

            patientService
                .IsUserInDbByIdAsync(dentistId)
                .Returns(true);

            patientService
                .IsUserInDbByIdAsync(model.PatientId)
                .Returns(true);

            patientService
                .IsUserDentistByIdAsync(dentistId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.EditProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureCreatorIsNotDentistValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditProcedureAsync_ShouldThrow_WhenProcedureIsNotFound()
        {
            // Arrange
            ProcedureCreateViewModel model = CreateTestProcedureCreateViewModel();
            model.ProcedureId = Guid.NewGuid();

            string dentistId = "dentist-id";

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 1, 1));

            patientService
                .IsUserInDbByIdAsync(dentistId)
                .Returns(true);

            patientService
                .IsUserInDbByIdAsync(model.PatientId)
                .Returns(true);

            patientService
                .IsUserDentistByIdAsync(dentistId)
                .Returns(true);

            procedureRepository
                .GetProcedureByIdAsync(model.ProcedureId.Value)
                .Returns((Procedure?)null);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                procedureService.EditProcedureAsync(model, dentistId));

            Assert.Equal(ProcedureCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditProcedureAsync_ShouldUpdateProcedureAndSaveChanges_WhenDataIsValid()
        {
            // Arrange
            ProcedureCreateViewModel model = CreateTestProcedureCreateViewModel();
            model.ProcedureId = Guid.NewGuid();
            model.PatientId = "new-patient-id";
            model.PatientPhoneNumber = "0888888888";
            model.ManipulationTypeId = Guid.NewGuid();
            model.DentistNote = "edited note";

            string dentistId = "dentist-id";

            Procedure procedure = new Procedure
            {
                ProcedureId = model.ProcedureId.Value,
                Date = new DateTime(2024, 1, 1),
                PatientId = "old-patient-id",
                DentistId = "old-dentist-id",
                PatientPhoneNumber = "0899999999",
                ManipulationTypeId = Guid.NewGuid(),
                Note = "old note"
            };

            manipulationService
                .ValidateManipulationTypesAsync(model.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 1, 1));

            patientService
                .IsUserInDbByIdAsync(dentistId)
                .Returns(true);

            patientService
                .IsUserInDbByIdAsync(model.PatientId)
                .Returns(true);

            patientService
                .IsUserDentistByIdAsync(dentistId)
                .Returns(true);

            procedureRepository
                .GetProcedureByIdAsync(model.ProcedureId.Value)
                .Returns(procedure);

            // Act
            await procedureService.EditProcedureAsync(model, dentistId);

            // Assert
            Assert.Equal(model.ProcedureDate, procedure.Date);
            Assert.Equal(model.PatientPhoneNumber, procedure.PatientPhoneNumber);
            Assert.Equal(model.ManipulationTypeId, procedure.ManipulationTypeId);
            Assert.Equal(model.DentistNote, procedure.Note);
            Assert.Equal(model.PatientId, procedure.PatientId);
            Assert.Equal(dentistId, procedure.DentistId);
        }



        private ProcedureCreateViewModel CreateTestProcedureCreateViewModel()
        {
            return new ProcedureCreateViewModel
            {
                PatientId = "patient-id",
                ProcedureDate = new DateTime(2026, 1, 1),
                PatientPhoneNumber = "0899999999",
                ManipulationTypeId = Guid.NewGuid(),
                DentistNote = "note"
            };
        }


    }
}


