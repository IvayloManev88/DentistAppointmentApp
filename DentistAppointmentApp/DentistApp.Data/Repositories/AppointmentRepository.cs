using DentistApp.Data.Models;
using DentistApp.Data.Repositories.Contracts;
using DentistApp.Data.Repositories.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DentistApp.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly DentistAppDbContext dbContext;

        public AppointmentRepository(DentistAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(Appointment appointment)
        {
            await dbContext.Appointments.AddAsync(appointment);
        }

        public async Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime, Guid? appointmentId = null)
        {
            return await dbContext.Appointments
            .AsNoTracking()
            .AnyAsync(a => a.Date == appointmentDateTime &&
            !a.IsDeleted && 
            (!appointmentId.HasValue || 
            a.AppointmentId != appointmentId.Value));
        }

        public async Task<AppointmentListingDto[]> GetAllAppotinmentsViewModelsAsync(string? userId = null)
        {
            IQueryable<Appointment> query = dbContext
             .Appointments
             .AsNoTracking();

            if (userId != null)
            {
                query = query.Where(a => a.PatientId == userId || a.DentistId == userId);
            }

            AppointmentListingDto[] appointments = await query
                .OrderBy(a => a.Date)
                .Select(a => new AppointmentListingDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientFirstName = a.Patient.FirstName,
                    PatientLastName = a.Patient.LastName,
                    DentistFirstName = a.Dentist.FirstName,
                    DentistLastName = a.Dentist.LastName,
                    AppointmentDate = a.Date,
                    PatientPhoneNumber = a.PatientPhoneNumber,
                    ManipulationName = a.ManipulationType.Name,
                    AppointmentNote = a.Note,
                    PatientId = a.PatientId,
                })
                .ToArrayAsync();

            return appointments;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
