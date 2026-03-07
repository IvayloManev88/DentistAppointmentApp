using DentistApp.ViewModels.AppointmentViewModels;
using DentistApp.ViewModels.FeedbackViewModels;

namespace DentistApp.Services.Core.Contracts
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackViewViewModel>> GetAllFeedbacksViewModelsAsync();
    }
}
