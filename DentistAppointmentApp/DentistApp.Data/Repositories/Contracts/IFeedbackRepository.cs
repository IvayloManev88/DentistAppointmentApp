namespace DentistApp.Data.Repositories.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Dtos;
    public interface IFeedbackRepository
    {
        Task<IEnumerable<FeedbackListingDto>> GetAllFeedbacksViewModelsAsync();

        Task AddAsync(Feedback feedbackToAdd);

        Task SaveChangesAsync();

        Task<bool> DoesFeedbackOnLastProcedureExist(Guid latestProcedureId);

        Task<double> GetAverageRatingAsync();
    }
}
