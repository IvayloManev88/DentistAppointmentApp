using DentistApp.ViewModels.AppointmentViewModels;
using DentistApp.ViewModels.ManipulationViewModels;

namespace DentistApp.Services.Core.Contracts
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentViewAppointmentViewModel>> GetAllAppotinmentsViewModelsAsync();

        Task<AppointmentCreateViewModel> CreateViewModelAsync();

        Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime);

        Task CreateAppointmentAsync(AppointmentCreateViewModel appointmentToCreate, DateTime appointmentDateTime, string userId);
    }
}
