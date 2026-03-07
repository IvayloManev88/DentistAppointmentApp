using DentistApp.ViewModels.AppointmentViewModels;
using DentistApp.ViewModels.FeedbackViewModels;
using DentistApp.ViewModels.ManipulationViewModels;

namespace DentistApp.Services.Core.Contracts
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackViewViewModel>> GetAllFeedbacksViewModelsAsync();

        Task CreateFeedbackAsync(FeedBackCreateViewModel feedbackToCreate, string patientId);
        Task<bool> CanUserLeaveFeedbackAsync(string patientId);
    }
}
