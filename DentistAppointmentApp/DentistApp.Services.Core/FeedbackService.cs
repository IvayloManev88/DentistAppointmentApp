namespace DentistApp.Services.Core
{
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.FeedbackViewModels;
    using static DentistApp.GCommon.ValidationMessages;

    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos;

    public class FeedbackService : IFeedbackService
    {
        private readonly IPatientService patientService;
        private readonly IProcedureService procedureService;
        private readonly IFeedbackRepository feedbackRepository;
        public FeedbackService(IPatientService patientService, IProcedureService procedureService, IFeedbackRepository feedbackRepository)
        {
            this.patientService = patientService;
            this.procedureService = procedureService;
            this.feedbackRepository= feedbackRepository;
        }
        public async Task<IEnumerable<FeedbackViewViewModel>> GetAllFeedbacksViewModelsAsync()
        {
            IEnumerable<FeedbackListingDto> feedbackDto = await feedbackRepository.GetAllFeedbacksViewModelsAsync();
            IEnumerable<FeedbackViewViewModel> feedbacks = feedbackDto
                .Select(f => new FeedbackViewViewModel
                {
                    CreatedOn = f.CreatedOn,
                    FeedbackText = f.FeedbackText,
                    Rating = f.Rating,
                    ProcedureManipulationType = f.ProcedureManipulationType,
                    ProcedurePatientName = f.ProcedurePatientName
                });
                
            return feedbacks;
        }


        public async Task CreateFeedbackAsync(FeedBackCreateViewModel feedbackToCreate, string patientId)
        {
            if (!await patientService.IsUserInDbByIdAsync(patientId))
            {
                throw new Exception(FeedbackUserNotInDatabase);
            }

            if (!await CanUserLeaveFeedbackAsync(patientId))
            {
                throw new Exception(FeedbackUserCannotLeaveFeedback);
            }
            Guid? latestProcedureId = await procedureService.GetLatestProcedureByUserIdAsync(patientId);
            if (latestProcedureId != null)
            {
                Feedback feedbackToAdd = new Feedback
                {
                    ProcedureId = latestProcedureId.Value,
                    CreatedOn = DateTime.UtcNow,
                    FeedbackText = feedbackToCreate.FeedbackText,
                    Rating = feedbackToCreate.Rating
                };
                await feedbackRepository.AddAsync(feedbackToAdd);
                await feedbackRepository.SaveChangesAsync();
            }
        }

        public async Task<bool> CanUserLeaveFeedbackAsync(string patientId)
        {
            Guid? latestProcedureId = await procedureService
                .GetLatestProcedureByUserIdAsync(patientId);

            if (latestProcedureId == null)
            {
                return false;
            }
            bool feedbackExists = await feedbackRepository
                .DoesFeedbackOnLastProcedureExist(latestProcedureId.Value);

            return !feedbackExists;
        }

        public async Task<double> GetAverageRatingAsync()
        {
            return await feedbackRepository
                .GetAverageRatingAsync();
        }
    }
}
