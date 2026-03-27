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
