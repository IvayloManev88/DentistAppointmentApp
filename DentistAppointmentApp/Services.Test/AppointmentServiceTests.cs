namespace DentistApp.Services.UnitTest
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using static DentistApp.GCommon.ValidationMessages;

    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.AppointmentViewModels;

    using NSubstitute;
    using Xunit;
    using DentistApp.ViewModels;

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

            appointmentService = new AppointmentService(null!, dateTimeService, manipulationService, patientService, appointmentRepository);
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
            await manipulationService.Received(1).GetManipulationTypesAsync();
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
            await patientService.Received(1).GetPatientsAsync();
        }

        [Fact]
        public async Task CreateViewModelAsync_ShouldNotLoadPatients_WhenCreatorIsNotDentist()
        {
            // Arrange
            
            // Act
            AppointmentCreateViewModel result =
                await appointmentService.CreateViewModelAsync("2026-03-28", "14:30", false);

            // Assert
            Assert.Empty(result.PatientsNames);
            await patientService.DidNotReceive().GetPatientsAsync();

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

            await manipulationService.Received(1).GetManipulationTypesAsync();
            await patientService.Received(1).GetPatientsAsync();

            dateTimeService.DidNotReceive().Today();
            dateTimeService.DidNotReceive().GetTime();
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
