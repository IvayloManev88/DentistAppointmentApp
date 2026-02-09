namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.ViewModels.AppointmentViewModels;
    using DentistApp.ViewModels.ManipulationViewModels;

    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentViewAppointmentViewModel>> GetAllAppotinmentsViewModelsAsync();

        Task<AppointmentCreateViewModel> CreateViewModelAsync();

        Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime);

        Task CreateAppointmentAsync(AppointmentCreateViewModel appointmentToCreate, DateTime appointmentDateTime, string userId);

        Task<Appointment?> GetAppointmentByIdAsync(Guid id);

        Task DeleteAppointmentByIdAsync(Guid id);

        Task<Appointment?> GetAppointmentToEditByUserIdAsync(Guid id, string userId);

        Task<AppointmentCreateViewModel> LoadEditViewModelByIdAsync(Guid id);

        Task EditAppointmentAsync(AppointmentCreateViewModel appointmentToEdit, Appointment editedAppointment, DateTime appointmentDateTime);
    }
}
