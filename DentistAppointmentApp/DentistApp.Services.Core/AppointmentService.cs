namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.AppointmentsScheduleViewModels;
    using DentistApp.ViewModels.AppointmentViewModels;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Globalization;
    using static DentistApp.GCommon.GlobalCommon;
    using static DentistApp.GCommon.ValidationMessages;
    using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<bool> AppointmentDuplicateDateAndTimeAsync(DateTime appointmentDateTime, Guid? appointmentId = null)
        {
            return await dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(a => a.Date == appointmentDateTime && !a.IsDeleted && (!appointmentId.HasValue || a.AppointmentId != appointmentId.Value));
        }

        public async Task CreateAppointmentAsync(AppointmentCreateViewModel appointmentToCreate, string userId)
        {
            bool isManipulationCorrect = await manipulationService.ValidateManipulationTypesAsync(appointmentToCreate.ManipulationTypeId);
            if (!isManipulationCorrect)
            {
                throw new Exception(ManipulationNotCorrectValidationMessage);
            }
            DateTime appointmentDateTime = appointmentToCreate.AppointmentDate.Date + appointmentToCreate.AppointmentTime;
            if (await this.AppointmentDuplicateDateAndTimeAsync(appointmentDateTime))
            {
                throw new Exception(DuplicatedAppointmentTimeValidationMessage);
            }
            if (appointmentDateTime < DateTime.Today)
            {
                throw new Exception(AppointmentCannotBeInThePastValidationMessage);
            }
            string? dentistId = await patientService.GetDentistIdAsync();
            if (dentistId == null)
            {
                throw new Exception(AppointmentCannotBeCreatedWithoutDentistValidationMessage);
            }
            if (!await patientService.IsUserInDbByIdAsync(userId))
            {
                throw new Exception(AppointmentUserNotInDatabaseValidationMessage);
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

        public async Task<AppointmentCreateViewModel> CreateViewModelAsync(string? selectedDate, string? selectedTime, bool isCreatorDentist=false)
        {
            AppointmentCreateViewModel createModel = new AppointmentCreateViewModel();
            if (!string.IsNullOrWhiteSpace(selectedDate) &&
     DateTime.TryParseExact(selectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
         DateTimeStyles.None, out DateTime parsedDate))
            {
                createModel.AppointmentDate = parsedDate.Date;
            }
            else
            {
                createModel.AppointmentDate = DateTime.Today;
            }

            if (!string.IsNullOrWhiteSpace(selectedTime) &&
                TimeSpan.TryParseExact(selectedTime, @"hh\:mm", CultureInfo.InvariantCulture,
                    out TimeSpan parsedTime))
            {
                createModel.AppointmentTime = parsedTime;
            }
            else
            {
                createModel.AppointmentTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            }   
            createModel.ManipulationTypes = await manipulationService.GetManipulationTypesAsync();

            if (isCreatorDentist)
            {
                createModel.PatientsNames = await patientService.GetPatientsAsync();
            }
            return createModel;

        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
        {
            return await dbContext
                .Appointments
                .SingleOrDefaultAsync(a => a.IsDeleted == false
                && a.AppointmentId == id);
        }

        public async Task<IEnumerable<AppointmentViewAppointmentViewModel>> GetAllAppotinmentsViewModelsAsync(string? user=null)
        {
            IQueryable<Appointment> query = dbContext
                .Appointments
                .AsNoTracking();

            if (user != null)
            {
                query = query.Where(a=>a.PatientId == user||a.DentistId==user);
            }

            IEnumerable<AppointmentViewAppointmentViewModel> appointments = await query
                .OrderBy(a => a.Date)
                .Select(a => new AppointmentViewAppointmentViewModel
                {
                    AppointmentId = a.AppointmentId.ToString(),
                    PatientAppointmentName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                    DentistAppointmentName = $"{a.Dentist.FirstName} {a.Dentist.LastName}",
                    AppointmentDate = a.Date.ToString(ApplicationDateTimeFormat, CultureInfo.InvariantCulture),
                    PatientAppointmentPhoneNumber = a.PatientPhoneNumber,
                    ManipulationName = a.ManipulationType.Name,
                    AppointmentNote = a.Note,
                    AppointmentUserCreated = a.PatientId.ToString()
                }).ToArrayAsync();
            return appointments;
        }

        public async Task DeleteAppointmentByIdAsync(Guid id)
        {
            Appointment? appointmentToDelete = await this.GetAppointmentByIdAsync(id);
            if (appointmentToDelete == null)
            {
                throw new Exception(AppointmentCannotBeFoundValidationMessage);
            }

            appointmentToDelete.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task<Appointment?> GetAppointmentToManipulateByUserIdAsync(Guid id, string userId)
        {
            Appointment? appointmentToEdit = await dbContext
                .Appointments
                .Where(a => a.PatientId == userId || a.DentistId == userId)
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.AppointmentId == id);
            return appointmentToEdit;
        }

        public async Task<AppointmentCreateViewModel> LoadEditViewModelByIdAsync(Guid id, bool isEditorDentist = false)
        {
            Appointment? appointmentToEdit = await this.GetAppointmentByIdAsync(id);
            if (appointmentToEdit == null)
            {
                throw new Exception(AppointmentCannotBeFoundValidationMessage);
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

            if (isEditorDentist)
            {
                editViewModel.PatientsNames = await patientService.GetPatientsAsync();
                editViewModel.PatientId = appointmentToEdit.PatientId;
            }
            return editViewModel;
        }
        public async Task EditAppointmentAsync(AppointmentCreateViewModel appointmentToEdit, string? patientId)
        {
            bool isManipulationCorrect = await manipulationService.ValidateManipulationTypesAsync(appointmentToEdit.ManipulationTypeId);
            if (!isManipulationCorrect)
            {
                throw new Exception(ManipulationNotCorrectValidationMessage);
            }
            DateTime appointmentDateTime = appointmentToEdit.AppointmentDate.Date + appointmentToEdit.AppointmentTime;
            if (await AppointmentDuplicateDateAndTimeAsync(appointmentDateTime, appointmentToEdit.AppointmentId))
            {
                throw new Exception(DuplicatedAppointmentTimeValidationMessage);
            }

            if (await this.AppointmentInFuture(appointmentDateTime))
            {
                throw new Exception(AppointmentCannotBeInThePastValidationMessage);
            }
            Appointment? editedAppointment = await this.GetAppointmentByIdAsync(appointmentToEdit.AppointmentId!.Value);
            if (editedAppointment == null)
            {
                throw new Exception(AppointmentCannotBeFoundValidationMessage);
            }
            editedAppointment.Date = appointmentDateTime;
            editedAppointment.PatientPhoneNumber = appointmentToEdit.PatientPhoneNumber;
            editedAppointment.ManipulationTypeId = appointmentToEdit.ManipulationTypeId;
            editedAppointment.Note = appointmentToEdit.Note;
            if (!string.IsNullOrWhiteSpace(patientId))
            {
                if (!await patientService.IsUserInDbByIdAsync(patientId))
                {
                    throw new Exception(AppointmentUserNotInDatabaseValidationMessage);
                }
                editedAppointment.PatientId = patientId;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> AppointmentInFuture(DateTime appointmentDateTime)
        {
            return appointmentDateTime < DateTime.Today;
        }

        public async Task<bool> CanAppointmentBeManipulatedByUserIdAsync(Guid id, string userId)
        {
            return await dbContext
                .Appointments
                .Where(a => a.PatientId == userId || a.DentistId == userId)
                .AnyAsync(a => a.IsDeleted == false && a.AppointmentId == id);
        }

        public async Task<WeeklyScheduleViewModel> GetWeeklyScheduleAsync(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;

            DateTime weekStartDate = date.AddDays(-diff).Date;
           

            DateTime weekEndDate = weekStartDate.AddDays(7);

            List<DateTime> appointments = await dbContext
                .Appointments
                .AsNoTracking()
                .Where(a => a.Date >= weekStartDate &&
                   a.Date < weekEndDate &&
                   !a.IsDeleted)
                .Select(a => a.Date)
                .ToListAsync();

            WeeklyScheduleViewModel model = new WeeklyScheduleViewModel
            {
                WeekStartDate = weekStartDate
            };

            for (int dayOffset = 0; dayOffset < 6; dayOffset++) // Monday-Saturday
            {
                DateTime currentDate = weekStartDate.AddDays(dayOffset);

                List<DateTime> dayAppointments = appointments
                    .Where(a => a.Date.Date == currentDate.Date)
                    .OrderBy(a => a)
                    .ToList();

                DayScheduleViewModel dayModel = new DayScheduleViewModel
                {
                    Date = currentDate,
                    Appointments = dayAppointments
                        .Select(a => new AppointmentScheduleItemViewModel
                        {
                            Start = a
                        })
                        .ToList()
                };

                for (int hour = WorkDayStart; hour < WorkDayEnd; hour++)
                {
                    DateTime slotStart = currentDate.AddHours(hour);

                    bool isTaken = dayAppointments.Any(a => a == slotStart);

                    if (!isTaken)
                    {
                        dayModel.FreeSlots.Add(new TimeSlotViewModel
                        {
                            Start = slotStart
                        });
                    }
                }
                model.Days.Add(dayModel);
            }
            return model;
        }

        public Task<IEnumerable<AppointmentViewAppointmentViewModel>> GetAllAppotinmentsForPatientAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
