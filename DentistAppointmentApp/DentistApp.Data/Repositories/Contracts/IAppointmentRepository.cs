namespace DentistApp.Data.Repositories.Contracts
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Dtos;

    public interface IAppointmentRepository
    {
        Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime, Guid? appointmentId = null);

        Task AddAsync(Appointment appointment);

        Task SaveChangesAsync();

        Task<AppointmentListingDto[]> GetAllAppotinmentsViewModelsAsync(string? userId = null);
    }
}
