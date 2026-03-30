namespace DentistApp.Services.UnitTest
{
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos;
    using static DentistApp.GCommon.ValidationMessages;

    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;
    
    using DentistApp.ViewModels.FeedbackViewModels;
    
    using NSubstitute;
    using DentistApp.Data.Models;

    public class FeedbackServiceTests
    {
        private readonly IFeedbackRepository feedbackRepository;
        private readonly IProcedureService procedureService;
        private readonly IPatientService patientService;

        private readonly FeedbackService feedbackService;

        public FeedbackServiceTests()
        {
            feedbackRepository = Substitute.For<IFeedbackRepository>();
            procedureService = Substitute.For<IProcedureService>();
            patientService = Substitute.For<IPatientService>();

            feedbackService = new FeedbackService(patientService, procedureService, feedbackRepository);
        }


        [Fact]
        public async Task GetAllFeedbacksViewModelsAsync_ShouldReturnMappedFeedbacks()
        {
            // Arrange
            IEnumerable<FeedbackListingDto> feedbackDtos = new List<FeedbackListingDto>
            {
                new FeedbackListingDto
                {
                CreatedOn = "01.03.2026",
                FeedbackText = "Very good service",
                Rating = 5,
                ProcedureManipulationType = "Teeth Cleaning",
                ProcedurePatientName = "Ivan Petrov"
                }
            };

            feedbackRepository
                .GetAllFeedbacksViewModelsAsync()
                .Returns(feedbackDtos);

            // Act
            IEnumerable<FeedbackViewViewModel> result = await feedbackService
                .GetAllFeedbacksViewModelsAsync();

            // Assert
            FeedbackViewViewModel[] resultArray = result.ToArray();

            Assert.Single(resultArray);

            Assert.Equal("01.03.2026", resultArray[0].CreatedOn);
            Assert.Equal("Very good service", resultArray[0].FeedbackText);
            Assert.Equal(5, resultArray[0].Rating);
            Assert.Equal("Teeth Cleaning", resultArray[0].ProcedureManipulationType);
            Assert.Equal("Ivan Petrov", resultArray[0].ProcedurePatientName);
        }

        [Fact]
        public async Task GetAllFeedbacksViewModelsAsync_ShouldReturnEmptyCollection_WhenNoFeedbacksExist()
        {
            // Arrange
            feedbackRepository
                .GetAllFeedbacksViewModelsAsync()
                .Returns(Enumerable.Empty<FeedbackListingDto>());

            // Act
            IEnumerable<FeedbackViewViewModel> result = await feedbackService
                .GetAllFeedbacksViewModelsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CanUserLeaveFeedbackAsync_ShouldReturnFalse_WhenNoProcedureExists()
        {
            // Arrange
            string patientId = "user1";

            procedureService
                .GetLatestProcedureByUserIdAsync(patientId)
                .Returns((Guid?)null);

            // Act
            bool result = await feedbackService.CanUserLeaveFeedbackAsync(patientId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanUserLeaveFeedbackAsync_ShouldReturnFalse_WhenFeedbackAlreadyExistsOnLatestProcedure()
        {
            // Arrange
            string patientId = "user1";
            Guid procedureId = Guid.NewGuid();

            procedureService
                .GetLatestProcedureByUserIdAsync(patientId)
                .Returns(procedureId);

            feedbackRepository
                .DoesFeedbackOnLastProcedureExist(procedureId)
                .Returns(true);

            // Act
            bool result = await feedbackService.CanUserLeaveFeedbackAsync(patientId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanUserLeaveFeedbackAsync_ShouldReturnTrue_WhenNoFeedbackExistsForLatestProcedure()
        {
            // Arrange
            string patientId = "user1";
            Guid procedureId = Guid.NewGuid();

            procedureService
                .GetLatestProcedureByUserIdAsync(patientId)
                .Returns(procedureId);

            feedbackRepository
                .DoesFeedbackOnLastProcedureExist(procedureId)
                .Returns(false);

            // Act
            bool result = await feedbackService.CanUserLeaveFeedbackAsync(patientId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldThrow_WhenUserIsNotInDatabase()
        {
            // Arrange
            FeedBackCreateViewModel model = new FeedBackCreateViewModel
            {
                FeedbackText = "Great service",
                Rating = 5
            };
            string patientId = "patient-1";

            patientService
                .IsUserInDbByIdAsync(patientId)
                .Returns(false);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                feedbackService.CreateFeedbackAsync(model, patientId));

            Assert.Equal(FeedbackUserNotInDatabase, ex.Message);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldThrow_WhenUserCannotLeaveFeedback()
        {
            // Arrange
            FeedBackCreateViewModel model = new FeedBackCreateViewModel
            {
                FeedbackText = "Great service",
                Rating = 5
            };
            string patientId = "patient-1";
            Guid procedureId = Guid.NewGuid();

            patientService
                .IsUserInDbByIdAsync(patientId)
                .Returns(true);

            procedureService
                .GetLatestProcedureByUserIdAsync(patientId)
                .Returns(procedureId);

            feedbackRepository
                .DoesFeedbackOnLastProcedureExist(procedureId)
                .Returns(true);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                feedbackService.CreateFeedbackAsync(model, patientId));

            Assert.Equal(FeedbackUserCannotLeaveFeedback, ex.Message);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldAddFeedbackAndSaveChanges_WhenInputIsValid()
        {
            // Arrange
            FeedBackCreateViewModel model = new FeedBackCreateViewModel
            {
                FeedbackText = "Excellent treatment",
                Rating = 5
            };
            string patientId = "patient-1";
            Guid procedureId = Guid.NewGuid();

            patientService
                .IsUserInDbByIdAsync(patientId)
                .Returns(true);

            procedureService
                .GetLatestProcedureByUserIdAsync(patientId)
                .Returns(procedureId);

            feedbackRepository
                .DoesFeedbackOnLastProcedureExist(procedureId)
                .Returns(false);

            // Act
            await feedbackService.CreateFeedbackAsync(model, patientId);

            // Assert
            await feedbackRepository.Received(1).AddAsync(
                Arg.Is<Feedback>(f =>
                    f.ProcedureId == procedureId &&
                    f.FeedbackText == model.FeedbackText &&
                    f.Rating == model.Rating
                ));

            await feedbackRepository.Received(1).SaveChangesAsync();
        }
    }
}



