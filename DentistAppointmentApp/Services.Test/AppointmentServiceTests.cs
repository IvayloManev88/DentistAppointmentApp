namespace DentistApp.Services.UnitTest
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.AppointmentViewModels;
    using NSubstitute;
    using System.Globalization;
    using Xunit;
    using static DentistApp.GCommon.ValidationMessages;
    using static DentistApp.GCommon.GlobalCommon;
    using DentistApp.Data.Repositories.Dtos.AppointmentDtos;

    public class AppointmentServiceTests
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        private readonly IDateTimeService dateTimeService;

        private readonly AppointmentService appointmentService;

        public AppointmentServiceTests()
        {
            appointmentRepository = Substitute.For<IAppointmentRepository>();
            manipulationService = Substitute.For<IManipulationService>();
            patientService = Substitute.For<IPatientService>();
            dateTimeService = Substitute.For<IDateTimeService>();

            appointmentService = new AppointmentService(dateTimeService, manipulationService, patientService, appointmentRepository);
        }



        [Fact]
        public async Task AppointmentDuplicateDateAndTime_ShouldReturnTrue_WhenDateTimeAreDuplicated()
        {
            //Arrange
            DateTime appointmentDateTime = new DateTime(2026, 3, 28, 10, 0, 0);
            Guid appointmentId = Guid.NewGuid();

            appointmentRepository.
                AppointmentDuplicateDateAndTimeAsync(appointmentDateTime, appointmentId)
                .Returns(true);

            //Act
            bool result = await appointmentService.AppointmentDuplicateDateAndTimeAsync(appointmentDateTime, appointmentId);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AppointmentDuplicateDateAndTime_ShouldReturnFalse_WhenDateTimeAreNotDuplicated()
        {
            //Arrange
            DateTime appointmentDateTime = new DateTime(2026, 3, 28, 10, 0, 0);
            Guid appointmentId = Guid.NewGuid();

            appointmentRepository.
                AppointmentDuplicateDateAndTimeAsync(appointmentDateTime, appointmentId)
                .Returns(false);

            //Act
            bool result = await appointmentService.AppointmentDuplicateDateAndTimeAsync(appointmentDateTime, appointmentId);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldThrow_WhenManipulationTypeIsInvalid()
        {
            //Arrange
            AppointmentCreateViewModel testModel = CreateValidModel();
            string userId = Guid.NewGuid().ToString();

            manipulationService
                .ValidateManipulationTypesAsync(testModel.ManipulationTypeId)
                .Returns(false);

            //Act

            //Assert

            Exception manipulationException = await Assert.ThrowsAsync<Exception>(() =>
            appointmentService.CreateAppointmentAsync(testModel, userId));

            Assert.Equal(ManipulationNotCorrectValidationMessage, manipulationException.Message);

        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldThrow_WhenAppointmentDuplicateDateAndTimeAsync()
        {
            //Arrange
            AppointmentCreateViewModel testModel = CreateValidModel();
            string userId = Guid.NewGuid().ToString();

            DateTime appointmentDate = testModel.AppointmentDate.Date + testModel.AppointmentTime;
            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(appointmentDate)
                .Returns(true);

            manipulationService
                .ValidateManipulationTypesAsync(testModel.ManipulationTypeId)
                .Returns(true);

            //Act

            //Assert
            Exception dateException = await Assert.ThrowsAsync<Exception>(() =>
            appointmentService.CreateAppointmentAsync(testModel, userId));

            Assert.Equal(DuplicatedAppointmentTimeValidationMessage, dateException.Message);
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldThrow_WhenAppointmentIsInThePast()
        {
            //Arrange
            AppointmentCreateViewModel testModel = CreateValidModel();
            string userId = Guid.NewGuid().ToString();

            DateTime appointmentDate = testModel.AppointmentDate.Date + testModel.AppointmentTime;
            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(appointmentDate)
                .Returns(false);

            manipulationService
                .ValidateManipulationTypesAsync(testModel.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 3, 29));

            //Act

            //Assert
            Exception pastException = await Assert.ThrowsAsync<Exception>(() =>
            appointmentService.CreateAppointmentAsync(testModel, userId));

            Assert.Equal(AppointmentCannotBeInThePastValidationMessage, pastException.Message);
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldThrow_WhenDentistDoesNotExist()
        {
            //Arrange
            AppointmentCreateViewModel testModel = CreateValidModel();
            string userId = Guid.NewGuid().ToString();

            DateTime appointmentDate = testModel.AppointmentDate.Date + testModel.AppointmentTime;
            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(appointmentDate)
                .Returns(false);

            manipulationService
                .ValidateManipulationTypesAsync(testModel.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 3, 27));

            patientService
                .GetDentistIdAsync()
                .Returns((string?)null);

            //Act

            //Assert
            Exception dentistException = await Assert.ThrowsAsync<Exception>(() =>
            appointmentService.CreateAppointmentAsync(testModel, userId));

            Assert.Equal(AppointmentCannotBeCreatedWithoutDentistValidationMessage, dentistException.Message);
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldThrow_WhenUserNotInDb()
        {
            //Arrange
            AppointmentCreateViewModel testModel = CreateValidModel();
            string userId = Guid.NewGuid().ToString();

            DateTime appointmentDate = testModel.AppointmentDate.Date + testModel.AppointmentTime;
            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(appointmentDate)
                .Returns(false);

            manipulationService
                .ValidateManipulationTypesAsync(testModel.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 3, 27));

            patientService
                .GetDentistIdAsync()
                .Returns(Guid.NewGuid().ToString());

            patientService
                .IsUserInDbByIdAsync(userId)
                .Returns(false);

            //Act

            //Assert
            Exception userException = await Assert.ThrowsAsync<Exception>(() =>
            appointmentService.CreateAppointmentAsync(testModel, userId));

            Assert.Equal(AppointmentUserNotInDatabaseValidationMessage, userException.Message);
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldAddAppointmentCorrectly()
        {
            //Arrange
            AppointmentCreateViewModel testModel = CreateValidModel();
            string userId = Guid.NewGuid().ToString();
            string dentistId = Guid.NewGuid().ToString();

            DateTime appointmentDate = testModel.AppointmentDate.Date + testModel.AppointmentTime;
            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(appointmentDate)
                .Returns(false);

            manipulationService
                .ValidateManipulationTypesAsync(testModel.ManipulationTypeId)
                .Returns(true);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 3, 27));

            patientService
                .GetDentistIdAsync()
                .Returns(dentistId);

            patientService
                .IsUserInDbByIdAsync(userId)
                .Returns(true);

            //Act
            await appointmentService.CreateAppointmentAsync(testModel, userId);

            //Assert

            await appointmentRepository.Received(1).AddAsync(Arg.Is<Appointment>(a =>
              a.PatientId == userId &&
              a.DentistId == dentistId &&
              a.Date == appointmentDate &&
              a.PatientPhoneNumber == testModel.PatientPhoneNumber &&
              a.ManipulationTypeId == testModel.ManipulationTypeId &&
              a.Note == testModel.Note
              ));

            await appointmentRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldUseCorrectDateTime_WhenValidInput()
        {
            //Arrange


            //Act
            AppointmentCreateViewModel result = await appointmentService.CreateViewModelAsync("2026-03-28", "14:30");

            //Assert
            Assert.Equal(new DateTime(2026, 3, 28), result.AppointmentDate);
            Assert.Equal(new TimeSpan(14, 30, 0), result.AppointmentTime);

        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldUseDateTimeNow_WhenInvalidInput()
        {
            //Arrange
            dateTimeService.Today().Returns(new DateTime(2026, 1, 1));
            dateTimeService.GetTime().Returns(new TimeSpan(10, 0, 0));

            //Act
            AppointmentCreateViewModel result = await appointmentService.CreateViewModelAsync("invalid", "invalid");

            //Assert
            Assert.Equal(new DateTime(2026, 1, 1), result.AppointmentDate);
            Assert.Equal(new TimeSpan(10, 0, 0), result.AppointmentTime);

        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldUseDateTimeNow_WhenInvalidTimeInput()
        {
            //Arrange

            dateTimeService.GetTime().Returns(new TimeSpan(10, 0, 0));
            DateTime correctDate = new DateTime(2025, 10, 1);

            //Act
            AppointmentCreateViewModel result = await appointmentService.CreateViewModelAsync("2025-10-01", "invalid");

            //Assert
            Assert.Equal(correctDate, result.AppointmentDate);
            Assert.Equal(new TimeSpan(10, 0, 0), result.AppointmentTime);

        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldUseDateTimeNow_WhenInvalidDateInput()
        {
            //Arrange
            dateTimeService.Today().Returns(new DateTime(2026, 1, 1));

            TimeSpan correctTime = new TimeSpan(9, 0, 0);

            //Act
            AppointmentCreateViewModel result = await appointmentService.CreateViewModelAsync("invalid", "09:00");

            //Assert
            Assert.Equal(new DateTime(2026, 1, 1), result.AppointmentDate);
            Assert.Equal(correctTime, result.AppointmentTime);

        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldLoadManipulationTypes()
        {
            // Arrange
            List<DropDown> manipulations = new()
        {
            new DropDown { Id = Guid.NewGuid(), Name = "Cleaning" },

        };

            manipulationService.GetManipulationTypesAsync().Returns(manipulations);

            // Act
            AppointmentCreateViewModel result =
                await appointmentService.CreateViewModelAsync("2026-03-28", "14:30");

            // Assert
            Assert.Equal(manipulations, result.ManipulationTypes);

        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldLoadPatients_WhenCreatorIsDentist()
        {
            // Arrange

            List<DropDown> patients = new()
        {
            new DropDown { Id = Guid.NewGuid(), Name = "Petar Ivanov" },

        };

            patientService.GetPatientsAsync().Returns(patients);

            // Act
            AppointmentCreateViewModel result =
                await appointmentService.CreateViewModelAsync("2026-03-28", "14:30", true);

            // Assert
            Assert.Equal(patients, result.PatientsNames);

        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldNotLoadPatients_WhenCreatorIsNotDentist()
        {
            // Arrange

            // Act
            AppointmentCreateViewModel result =
                await appointmentService.CreateViewModelAsync("2026-03-28", "14:30", false);

            // Assert
            Assert.NotNull(result.PatientsNames);
            Assert.Empty(result.PatientsNames);


        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldReturnCorrectViewModel_WhenInputIsValid_AndCreatorIsDentist()
        {
            // Arrange
            DateTime expectedDate = new DateTime(2026, 3, 28);
            TimeSpan expectedTime = new TimeSpan(14, 30, 0);


            List<DropDown> expectedManipulations = new List<DropDown>
        {
            new DropDown { Id = Guid.NewGuid(), Name = "Check-up" },
        };

            List<DropDown> expectedPatients = new List<DropDown>
        {
            new DropDown { Id = Guid.NewGuid(), Name = "Ivan Ivanov" },
        };

            manipulationService
                .GetManipulationTypesAsync()
                .Returns(expectedManipulations);

            patientService
                .GetPatientsAsync()
                .Returns(expectedPatients);

            // Act
            AppointmentCreateViewModel result =
                await appointmentService.CreateViewModelAsync("2026-03-28", "14:30", true);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(expectedDate, result.AppointmentDate);
            Assert.Equal(expectedTime, result.AppointmentTime);

            Assert.Equal(expectedManipulations, result.ManipulationTypes);
            Assert.Equal(expectedPatients, result.PatientsNames);
        }

        [Fact]
        public async Task GetAllAppotinmentsViewModelsAsync_ShouldMapDtosCorrectly()
        {
            //Arrange
            Guid appointmentId = Guid.NewGuid();
            string patientId = "testPatient";

            AppointmentListingDto[] appointmentsListings = new AppointmentListingDto[] {
                new AppointmentListingDto
                {
                    AppointmentId = appointmentId,
                    PatientFirstName = "Ivan",
                    PatientLastName = "Petrov",
                    DentistFirstName = "Kristina",
                    DentistLastName = "Maneva",
                    AppointmentDate = new DateTime(2026, 3, 28, 14, 30, 0),
                    PatientPhoneNumber = "0876720270",
                    ManipulationName = "Cleaning",
                    AppointmentNote = "Test note",
                    PatientId = patientId
                }
            };

            appointmentRepository
                .GetAllAppotinmentsViewModelsAsync(null)
                .Returns(appointmentsListings);

            // Act
            IEnumerable<AppointmentViewAppointmentViewModel> result =
                await appointmentService.GetAllAppotinmentsViewModelsAsync();

            // Assert
            AppointmentViewAppointmentViewModel model = Assert.Single(result);

            Assert.Equal(appointmentId.ToString(), model.AppointmentId);
            Assert.Equal("Ivan Petrov", model.PatientAppointmentName);
            Assert.Equal("Kristina Maneva", model.DentistAppointmentName);
            Assert.Equal(
                new DateTime(2026, 3, 28, 14, 30, 0).ToString(ApplicationDateTimeFormat, CultureInfo.InvariantCulture),
                model.AppointmentDate);
            Assert.Equal("0876720270", model.PatientAppointmentPhoneNumber);
            Assert.Equal("Cleaning", model.ManipulationName);
            Assert.Equal("Test note", model.AppointmentNote);
            Assert.Equal(patientId, model.AppointmentUserCreated);
        }

        [Fact]
        public async Task GetAllAppotinmentsViewModelsAsync_ShouldReturnEmptyCollection_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            appointmentRepository
                .GetAllAppotinmentsViewModelsAsync(null)
                .Returns(Array.Empty<AppointmentListingDto>());

            // Act
            IEnumerable<AppointmentViewAppointmentViewModel> result =
                await appointmentService.GetAllAppotinmentsViewModelsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await appointmentRepository.Received(1).GetAllAppotinmentsViewModelsAsync(null);
        }

        [Fact]
        public async Task DeleteAppointmentByIdAsync_ShouldSoftDeleteAndSave_WhenAppointmentExists()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();

            Appointment appointment = new Appointment
            {
                AppointmentId = appointmentId,
                IsDeleted = false
            };

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns(appointment);

            // Act
            await appointmentService.DeleteAppointmentByIdAsync(appointmentId);

            // Assert
            await appointmentRepository.Received(1).GetAppointmentByIdAsync(appointmentId);
            await appointmentRepository.Received(1).SoftDeleteAppointmentAsync(appointment);
            await appointmentRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAppointmentByIdAsync_ShouldThrow_WhenAppointmentDoesNotExist()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns((Appointment?)null);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.DeleteAppointmentByIdAsync(appointmentId));

            Assert.Equal(AppointmentCannotBeFoundValidationMessage, ex.Message);

            await appointmentRepository.Received(1).GetAppointmentByIdAsync(appointmentId);
            await appointmentRepository.DidNotReceive().SoftDeleteAppointmentAsync(Arg.Any<Appointment>());
            await appointmentRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task LoadEditViewModelByIdAsync_ShouldReturnMappedViewModel_WhenAppointmentExists_AndEditorIsNotDentist()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2026, 4, 2, 14, 30, 0);
            Guid manipulationTypeId = Guid.NewGuid();

            Appointment appointment = new Appointment
            {
                AppointmentId = appointmentId,
                Date = appointmentDate,
                PatientPhoneNumber = "0888123456",
                ManipulationTypeId = manipulationTypeId,
                Note = "Test note",
                PatientId = "testPatient"
            };

            List<DropDown> manipulationTypes = new()
            {
               new DropDown { Id = manipulationTypeId, Name = "Cleaning" }
            };

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns(appointment);

            manipulationService
                .GetManipulationTypesAsync()
                .Returns(manipulationTypes);

            // Act
            AppointmentCreateViewModel result =
                await appointmentService.LoadEditViewModelByIdAsync(appointmentId, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentId, result.AppointmentId);
            Assert.Equal(appointmentDate.Date, result.AppointmentDate);
            Assert.Equal(appointmentDate.TimeOfDay, result.AppointmentTime);
            Assert.Equal("0888123456", result.PatientPhoneNumber);
            Assert.Equal(manipulationTypeId, result.ManipulationTypeId);
            Assert.Equal("Test note", result.Note);
            Assert.Equal(manipulationTypes, result.ManipulationTypes);
            Assert.NotNull(result.PatientsNames);
            Assert.Empty(result.PatientsNames);
            Assert.Null(result.PatientId);
        }

        [Fact]
        public async Task LoadEditViewModelByIdAsync_ShouldReturnMappedViewModelWithPatients_WhenEditorIsDentist()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2026, 4, 2, 14, 30, 0);
            Guid manipulationTypeId = Guid.NewGuid();
            Guid patient1Id = Guid.NewGuid();
            Guid patient2Id = Guid.NewGuid();

            Appointment appointment = new Appointment
            {
                AppointmentId = appointmentId,
                Date = appointmentDate,
                PatientPhoneNumber = "0888123456",
                ManipulationTypeId = manipulationTypeId,
                Note = "Test note",
                PatientId = patient1Id.ToString()
            };

            List<DropDown> manipulationTypes = new()
            {
                new DropDown { Id = manipulationTypeId, Name = "Cleaning" }
            };
                        

            List<DropDown> patients = new()
            {
                new DropDown { Id = patient1Id, Name = "Ivan Petrov" },
                new DropDown { Id = patient2Id, Name = "Maria Ivanova" }
            };

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns(appointment);

            manipulationService
                .GetManipulationTypesAsync()
                .Returns(manipulationTypes);

            patientService
                .GetPatientsAsync()
                .Returns(patients);

            // Act
            AppointmentCreateViewModel result =
                await appointmentService.LoadEditViewModelByIdAsync(appointmentId, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentId, result.AppointmentId);
            Assert.Equal(appointmentDate.Date, result.AppointmentDate);
            Assert.Equal(appointmentDate.TimeOfDay, result.AppointmentTime);
            Assert.Equal("0888123456", result.PatientPhoneNumber);
            Assert.Equal(manipulationTypeId, result.ManipulationTypeId);
            Assert.Equal("Test note", result.Note);
            Assert.Equal(manipulationTypes, result.ManipulationTypes);
            Assert.Equal(patients, result.PatientsNames);
            Assert.Equal(patient1Id.ToString(), result.PatientId);
        }

        [Fact]
        public async Task LoadEditViewModelByIdAsync_ShouldThrow_WhenAppointmentDoesNotExist()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns((Appointment?)null);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.LoadEditViewModelByIdAsync(appointmentId, false));

            Assert.Equal(AppointmentCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditAppointmentAsync_ShouldEditAppointmentAndSave_WhenDataIsValid()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            Guid manipulationTypeId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2026, 4, 10);
            TimeSpan appointmentTime = new TimeSpan(14, 30, 0);
            DateTime expectedDateTime = appointmentDate.Date + appointmentTime;
            string newPatientId = "patient-2";

            AppointmentCreateViewModel viewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                PatientPhoneNumber = "0888123456",
                ManipulationTypeId = manipulationTypeId,
                Note = "Edited note"
            };

            Appointment appointment = new Appointment
            {
                AppointmentId = appointmentId,
                Date = new DateTime(2026, 4, 5, 10, 0, 0),
                PatientPhoneNumber = "0899999999",
                ManipulationTypeId = Guid.NewGuid(),
                Note = "Old note",
                PatientId = "patient-1"
            };

            manipulationService
                .ValidateManipulationTypesAsync(manipulationTypeId)
                .Returns(true);

            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(expectedDateTime, appointmentId)
                .Returns(false);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 4, 1));

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns(appointment);

            patientService
                .IsUserInDbByIdAsync(newPatientId)
                .Returns(true);

            // Act
            await appointmentService.EditAppointmentAsync(viewModel, newPatientId);

            // Assert
            Assert.Equal(expectedDateTime, appointment.Date);
            Assert.Equal("0888123456", appointment.PatientPhoneNumber);
            Assert.Equal(manipulationTypeId, appointment.ManipulationTypeId);
            Assert.Equal("Edited note", appointment.Note);
            Assert.Equal(newPatientId, appointment.PatientId);
        }

        [Fact]
        public async Task EditAppointmentAsync_ShouldThrow_WhenManipulationIsInvalid()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            Guid manipulationTypeId = Guid.NewGuid();

            AppointmentCreateViewModel viewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentId,
                AppointmentDate = new DateTime(2026, 4, 10),
                AppointmentTime = new TimeSpan(14, 30, 0),
                ManipulationTypeId = manipulationTypeId
            };

            manipulationService
                .ValidateManipulationTypesAsync(manipulationTypeId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.EditAppointmentAsync(viewModel, "patient-1"));

            Assert.Equal(ManipulationNotCorrectValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditAppointmentAsync_ShouldThrow_WhenAppointmentDateTimeIsDuplicated()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            Guid manipulationTypeId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2026, 4, 10);
            TimeSpan appointmentTime = new TimeSpan(14, 30, 0);
            DateTime expectedDateTime = appointmentDate.Date + appointmentTime;

            AppointmentCreateViewModel viewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                ManipulationTypeId = manipulationTypeId
            };

            manipulationService
                .ValidateManipulationTypesAsync(manipulationTypeId)
                .Returns(true);

            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(expectedDateTime, appointmentId)
                .Returns(true);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.EditAppointmentAsync(viewModel, "patient-1"));

            Assert.Equal(DuplicatedAppointmentTimeValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditAppointmentAsync_ShouldThrow_WhenAppointmentIsInThePast()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            Guid manipulationTypeId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2025, 4, 1);
            TimeSpan appointmentTime = new TimeSpan(10, 0, 0);
            DateTime expectedDateTime = appointmentDate.Date + appointmentTime;

            AppointmentCreateViewModel viewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                ManipulationTypeId = manipulationTypeId
            };

            manipulationService
                .ValidateManipulationTypesAsync(manipulationTypeId)
                .Returns(true);

            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(expectedDateTime, appointmentId)
                .Returns(false);

            dateTimeService
                .Today()
                .Returns(new DateTime(2025, 4, 2));

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.EditAppointmentAsync(viewModel, "patient-1"));

            Assert.Equal(AppointmentCannotBeInThePastValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditAppointmentAsync_ShouldThrow_WhenAppointmentIdIsNull()
        {
            // Arrange
            Guid manipulationTypeId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2026, 4, 10);
            TimeSpan appointmentTime = new TimeSpan(14, 30, 0);
            DateTime expectedDateTime = appointmentDate.Date + appointmentTime;

            AppointmentCreateViewModel viewModel = new AppointmentCreateViewModel
            {
                AppointmentId = null,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                ManipulationTypeId = manipulationTypeId
            };

            manipulationService
                .ValidateManipulationTypesAsync(manipulationTypeId)
                .Returns(true);

            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(expectedDateTime, null)
                .Returns(false);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 4, 1));

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.EditAppointmentAsync(viewModel, "patient-1"));

            Assert.Equal(AppointmentCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditAppointmentAsync_ShouldThrow_WhenAppointmentDoesNotExist()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            Guid manipulationTypeId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2026, 4, 10);
            TimeSpan appointmentTime = new TimeSpan(14, 30, 0);
            DateTime expectedDateTime = appointmentDate.Date + appointmentTime;

            AppointmentCreateViewModel viewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                ManipulationTypeId = manipulationTypeId
            };

            manipulationService
                .ValidateManipulationTypesAsync(manipulationTypeId)
                .Returns(true);

            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(expectedDateTime, appointmentId)
                .Returns(false);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 4, 1));

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns((Appointment?)null);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.EditAppointmentAsync(viewModel, "patient-1"));

            Assert.Equal(AppointmentCannotBeFoundValidationMessage, ex.Message);
        }

        [Fact]
        public async Task EditAppointmentAsync_ShouldThrow_WhenPatientIdIsNotInDatabase()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            Guid manipulationTypeId = Guid.NewGuid();
            DateTime appointmentDate = new DateTime(2026, 4, 10);
            TimeSpan appointmentTime = new TimeSpan(14, 30, 0);
            DateTime expectedDateTime = appointmentDate.Date + appointmentTime;
            string patientId = "missing-patient";

            AppointmentCreateViewModel viewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentId,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                PatientPhoneNumber = "0888123456",
                ManipulationTypeId = manipulationTypeId,
                Note = "Edited note"
            };

            Appointment appointment = new Appointment
            {
                AppointmentId = appointmentId,
                Date = new DateTime(2026, 4, 5, 10, 0, 0),
                PatientPhoneNumber = "0899999999",
                ManipulationTypeId = Guid.NewGuid(),
                Note = "Old note",
                PatientId = "patient-1"
            };

            manipulationService
                .ValidateManipulationTypesAsync(manipulationTypeId)
                .Returns(true);

            appointmentRepository
                .AppointmentDuplicateDateAndTimeAsync(expectedDateTime, appointmentId)
                .Returns(false);

            dateTimeService
                .Today()
                .Returns(new DateTime(2026, 4, 1));

            appointmentRepository
                .GetAppointmentByIdAsync(appointmentId)
                .Returns(appointment);

            patientService
                .IsUserInDbByIdAsync(patientId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(
                () => appointmentService.EditAppointmentAsync(viewModel, patientId));

            Assert.Equal(AppointmentUserNotInDatabaseValidationMessage, ex.Message);
        }


        [Fact]
        public async Task AppointmentInFuture_ShouldReturnTrue_AppointmentInPast()
        {
            // Arrange
            dateTimeService.Today()
                .Returns(new DateTime(2026, 1, 1));

            DateTime dateTimeAppointment = new DateTime(2025, 1, 1);
            // Act
            bool result = await appointmentService.IsAppointmentNotInFuture(dateTimeAppointment);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AppointmentInFuture_ShouldReturnFalse_AppointmentNotInThePast()
        {
            // Arrange
            dateTimeService.Today()
                .Returns(new DateTime(2026, 1, 1));

            DateTime dateTimeAppointment = new DateTime(2027, 1, 1);
            // Act
            bool result = await appointmentService.IsAppointmentNotInFuture(dateTimeAppointment);
            // Assert
            Assert.False(result);
        }

        private static AppointmentCreateViewModel CreateValidModel()
        {
            return new AppointmentCreateViewModel
            {
                AppointmentDate = new DateTime(2026, 3, 28),
                AppointmentTime = new TimeSpan(10, 0, 0),
                ManipulationTypeId = Guid.NewGuid(),
                PatientPhoneNumber = "0876720270",
                Note = "Testvam"
            };
        }
           
    }
}
