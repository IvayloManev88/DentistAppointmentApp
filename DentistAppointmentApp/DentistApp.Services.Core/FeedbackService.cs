namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.FeedbackViewModels;
    using static DentistApp.GCommon.GlobalCommon;
    using static DentistApp.GCommon.ValidationMessages;

    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using DentistApp.Data.Models;

    public class FeedbackService : IFeedbackService
    {
        private readonly DentistAppDbContext dbContext;
        private readonly PatientService patientService;
        private readonly ProcedureService procedureService;
        public FeedbackService(DentistAppDbContext dbContext, PatientService patientService, ProcedureService procedureService)
        {
            this.dbContext = dbContext;
            this.patientService = patientService;
            this.procedureService = procedureService;
        }
        public async Task<IEnumerable<FeedbackViewViewModel>> GetAllFeedbacksViewModelsAsync()
        {
            IEnumerable<FeedbackViewViewModel> feedbacks = await dbContext
                .Feedbacks
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedOn)
                .Select(f => new FeedbackViewViewModel
                {
                    CreatedOn = f.CreatedOn.ToString(ApplicationDateTimeFormat, CultureInfo.InvariantCulture),
                    FeedbackText = f.FeedbackText,
                    Rating = f.Rating,
                    ProcedureManipulationType = f.Procedure.ManipulationType.Name,
                    ProcedurePatientName = f.Procedure.Patient.FirstName + " " + f.Procedure.Patient.LastName.Substring(0, 1) + "."
                })
                .ToArrayAsync();
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
                await dbContext.Feedbacks.AddAsync(feedbackToAdd);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> CanUserLeaveFeedbackAsync(string patientId)
        {
            Guid? latestProcedureId = await procedureService.GetLatestProcedureByUserIdAsync(patientId);
            if (latestProcedureId == null)
            {
                return false;
            }
            bool feedbackExists = await dbContext.Feedbacks
            .AsNoTracking()
            .AnyAsync(f => f.ProcedureId == latestProcedureId);

            return !feedbackExists;
        }
    }
}
