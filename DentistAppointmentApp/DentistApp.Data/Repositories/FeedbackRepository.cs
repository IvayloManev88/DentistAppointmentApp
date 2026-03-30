

namespace DentistApp.Data.Repositories
{
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Data.Repositories.Dtos;
    using static DentistApp.GCommon.GlobalCommon;
    using DentistApp.ViewModels.FeedbackViewModels;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using DentistApp.Data.Models;

    public class FeedbackRepository:IFeedbackRepository
    {
        private readonly DentistAppDbContext dbContext;

        public FeedbackRepository(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<FeedbackListingDto>> GetAllFeedbacksViewModelsAsync()
        {
            return await dbContext
                .Feedbacks
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedOn)
                .Select(f => new FeedbackListingDto
                {
                    CreatedOn = f.CreatedOn.ToString(ApplicationDateTimeFormat, CultureInfo.InvariantCulture),
                    FeedbackText = f.FeedbackText,
                    Rating = f.Rating,
                    ProcedureManipulationType = f.Procedure.ManipulationType.Name,
                    ProcedurePatientName = f.Procedure.Patient.FirstName + " " + f.Procedure.Patient.LastName.Substring(0, 1) + "."
                })
                .ToArrayAsync();
        }

        public async Task AddAsync(Feedback feedbackToAdd)
        {
            await dbContext.Feedbacks.AddAsync(feedbackToAdd);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> DoesFeedbackOnLastProcedureExist(Guid latestProcedureId)
        {
            return await dbContext.Feedbacks
            .AsNoTracking()
            .AnyAsync(f => f.ProcedureId == latestProcedureId);
        }

        public async Task<double> GetAverageRatingAsync()
        {
            return await dbContext.Feedbacks
                .AsNoTracking()
                .AverageAsync(f => f.Rating);
        }
    }
}
