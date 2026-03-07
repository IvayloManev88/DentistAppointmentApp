namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.FeedbackViewModels;
    using static DentistApp.GCommon.GlobalCommon;

    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    public class FeedbackService : IFeedbackService
    {
        private readonly DentistAppDbContext dbContext;
        public FeedbackService(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<FeedbackViewViewModel>> GetAllFeedbacksViewModelsAsync()
        {
            IEnumerable<FeedbackViewViewModel> feedbacks = await dbContext
                .Feedbacks
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedOn)
                .Select(f => new FeedbackViewViewModel
                {
                    CreatedOn = f.CreatedOn.ToString(ApplicationDateTimeFormat,CultureInfo.InvariantCulture),
                    FeedbackText = f.FeedbackText,
                    Rating = f.Rating,
                    ProcedureManipulationType = f.Procedure.ManipulationType.Name,
                    ProcedurePatientName = f.Procedure.Patient.FirstName + " " + f.Procedure.Patient.LastName.Substring(0, 1) + "."
                })
                .ToArrayAsync();
            return feedbacks;
        }
    }
}
