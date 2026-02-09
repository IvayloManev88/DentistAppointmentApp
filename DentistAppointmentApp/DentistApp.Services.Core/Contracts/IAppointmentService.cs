namespace DentistApp.Services.Core.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.ViewModels.AppointmentViewModels;

    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentViewAppointmentViewModel>> GetAllAppotinmentsViewModelsAsync();

        Task<AppointmentCreateViewModel> CreateViewModelAsync();

        Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime,Guid? appointmentId= null);

        bool AppointmentInFuture(DateTime appointmentDateTime);

        Task CreateAppointmentAsync(AppointmentCreateViewModel appointmentToCreate, string userId);

        Task<Appointment?> GetAppointmentByIdAsync(Guid id);

        Task DeleteAppointmentByIdAsync(Guid id);

        Task<Appointment?> GetAppointmentToManipulateByUserIdAsync(Guid id, string userId);

        bool CanAppointmentBeManipulatedByUserIdAsync(Guid id, string userId);

        Task<AppointmentCreateViewModel> LoadEditViewModelByIdAsync(Guid id);

        Task EditAppointmentAsync(AppointmentCreateViewModel appointmentToEdit);
    }
}
