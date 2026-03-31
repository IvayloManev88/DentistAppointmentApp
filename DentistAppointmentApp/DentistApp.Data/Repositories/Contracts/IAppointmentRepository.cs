namespace DentistApp.Data.Repositories.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Dtos.AppointmentDtos;

    public interface IAppointmentRepository
    {
        Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime, Guid? appointmentId = null);

        Task AddAsync(Appointment appointment);

        Task SaveChangesAsync();

        Task<AppointmentListingDto[]> GetAllAppotinmentsViewModelsAsync(string? userId = null);

        Task<Appointment?> GetAppointmentByIdAsync(Guid id);

        Task SoftDeleteAppointmentAsync(Appointment appointment);

        Task<Appointment?> GetAppointmentToManipulateByUserIdAsync(Guid id, string userId);

        Task<bool> CanAppointmentBeManipulatedByUserIdAsync(Guid id, string userId);

        Task<List<DateTime>> GetAppointmentsAsDateTimeList(DateTime weekStartDate, DateTime weekEndDate);
    }
}
