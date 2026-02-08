using DentistApp.Data;
using DentistApp.Services.Core.Contracts;
using DentistApp.ViewModels.AppointmentViewModels;
namespace DentistApp.Services.Core
{
    using DentistApp.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using System.Globalization;

    using static DentistApp.GCommon.GlobalCommon;
    public class AppointmentService : IAppointmentService
    {
        private readonly DentistAppDbContext dbContext;
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        public AppointmentService(DentistAppDbContext dbContext, IManipulationService manipulationService, IPatientService patientService)
        {
            this.dbContext = dbContext;
            this.manipulationService = manipulationService;
            this.patientService = patientService;
        }

        public async Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime)
        {
            return await this.dbContext.Appointments.AsNoTracking().AnyAsync(a => a.Date == appointmentDateTime && a.IsDeleted == false);
        }

        public async Task CreateAppointmentAsync(AppointmentCreateViewModel appointmentToCreate, DateTime appointmentDateTime, string userId)
        {
            bool isManipulationCorrect = await manipulationService.ValidateManipulationTypesAsync(appointmentToCreate.ManipulationTypeId);
            if (!isManipulationCorrect)
            {
                throw new Exception("Manipulation Service is not correct");
            }
            if (await this.AppointmentDuplicateDateAndTimeAsync(appointmentDateTime))
            {
                throw new Exception("Duplicated Appointment Date/Time");
            }
            if (appointmentDateTime < DateTime.Today)
            {
                throw new Exception("Appointment's Date and Time combination cannot be in the past");
            }
            string? dentistId = await patientService.GetDentistIdAsync();
            if (dentistId == null)
            {
                throw new Exception("Error while creating Appointment. At least one dentist user should be configured");
            }

            Appointment currentAppointment = new Appointment
            {
                PatientId = userId,
                DentistId = dentistId,
                Date = appointmentDateTime,
                PatientPhoneNumber = appointmentToCreate.PatientPhoneNumber,
                ManipulationTypeId = appointmentToCreate.ManipulationTypeId,
                Note = appointmentToCreate.Note

            };

            await dbContext.Appointments.AddAsync(currentAppointment);
            await dbContext.SaveChangesAsync();

        }

        public async Task<AppointmentCreateViewModel> CreateViewModelAsync()
        {
            AppointmentCreateViewModel createModel = new AppointmentCreateViewModel();
            createModel.AppointmentDate = DateTime.Today;
            createModel.AppointmentTime = DateTime.Now.TimeOfDay;
            createModel.ManipulationTypes = await manipulationService.GetManipulationTypesAsync();
            return createModel;

        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
        {
            return await dbContext
                .Appointments
                .SingleOrDefaultAsync(a => a.IsDeleted == false 
                && a.AppointmentId == id);
        }

        public async Task<IEnumerable<AppointmentViewAppointmentViewModel>> GetAllAppotinmentsViewModelsAsync()
        {
            IEnumerable<AppointmentViewAppointmentViewModel> appointments = await dbContext
                .Appointments
                .AsNoTracking()
                .OrderBy(a => a.Date)
                .Select(a => new AppointmentViewAppointmentViewModel
                {
                    AppointmentId = a.AppointmentId.ToString(),
                    PatientAppointmentName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                    DentistAppointmentName = $"{a.Dentist.FirstName} {a.Dentist.LastName}",
                    AppointmentDate = a.Date.ToString(ApplicationDateTimeFormat,CultureInfo.InvariantCulture),
                    PatientAppointmentPhoneNumber = a.PatientPhoneNumber,
                    ManipulationName = a.ManipulationType.Name,
                    AppointmentNote = a.Note
                }).ToArrayAsync();
            return appointments;
        }

        public async Task DeleteAppointmentByIdAsync(Guid id)
        {
            Appointment? appointmentToDelete =await this.GetAppointmentByIdAsync(id);
            if (appointmentToDelete == null)
            {
                throw new Exception("Appointment to Delete not found");
            }

            appointmentToDelete.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task<Appointment?> GetAppointmentToEditByUserIdAsync(Guid id, string userId)
        {
            Appointment? appointmentToEdit = await dbContext
                .Appointments
                .Where(a => a.PatientId == userId || a.DentistId == userId)
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.AppointmentId == id);
            return appointmentToEdit;
        }

        public async Task<AppointmentCreateViewModel> LoadEditViewModelByIdAsync(Guid id)
        {
            Appointment? appointmentToEdit =await this.GetAppointmentByIdAsync(id);
            if (appointmentToEdit == null)
            {
                throw new Exception("Appointment to Edit not found");
            }

            AppointmentCreateViewModel editViewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentToEdit.AppointmentId,
                AppointmentDate = appointmentToEdit.Date.Date,
                AppointmentTime = appointmentToEdit.Date.TimeOfDay,
                PatientPhoneNumber = appointmentToEdit.PatientPhoneNumber,
                ManipulationTypeId = appointmentToEdit.ManipulationTypeId,
                Note = appointmentToEdit.Note,
                ManipulationTypes = await manipulationService.GetManipulationTypesAsync()
            };
            return editViewModel;
        }
    }
}
