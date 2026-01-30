namespace DentistApp.Web.Controllers
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.Services.Core.Models;
    using DentistApp.Web.ViewModels.AppointmentViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;

    using static GCommon.AppointmentConstants;
    using static GCommon.GlobalCommon;
    
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DentistAppDbContext dbContext;
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        
        public AppointmentController(UserManager<ApplicationUser> userManager, DentistAppDbContext dbContext, IManipulationService manipulationService, IPatientService patientService)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.manipulationService = manipulationService;
            this.patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            AppointmentViewAppointmentViewModel[] appointments = await dbContext
                .Appointments
                .AsNoTracking()
                .OrderBy(a => a.Date)
                .Select(a => new AppointmentViewAppointmentViewModel
                {
                    AppointmentId = a.AppointmentId.ToString(),
                    PatientAppointmentName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                    DentistAppointmentName = $"{a.Dentist.FirstName} {a.Dentist.LastName}",
                    AppointmentDate = a.Date.ToString(DateTimeFormat),
                    PatientAppointmentPhoneNumber = a.PatientPhoneNumber,
                    ManipulationName = a.ManipulationType.Name,
                    AppointmentNote = a.Note

                }).ToArrayAsync();
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<LookupItem> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            AppointmentCreateViewModel createModel = new AppointmentCreateViewModel();
            createModel.AppointmentDate = DateTime.Today;
            createModel.AppointmentTime = DateTime.Now.TimeOfDay;
            await PopulateManipulationTypesAsync(createModel);
            return View(createModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateViewModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createModel);
            }

            DateTime appointmentDate = createModel.AppointmentDate.Date + createModel.AppointmentTime;

            if (!ValidateManipulationId(dbContext,createModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(createModel.ManipulationTypeId), "The selected manipulation is incorrect");
                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }          

            if (await this.dbContext.Appointments.AsNoTracking().AnyAsync(a => a.Date == appointmentDate && a.IsDeleted == false))
            {
                ModelState
                    .AddModelError(nameof(createModel.AppointmentDate), "The selected combination Date/Time is already taken. Please try different Date/Time");
                

                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }

            if (appointmentDate < DateTime.Today)
            {
                ModelState
                   .AddModelError(nameof(createModel.AppointmentDate), "You should not set an appintment in the past");
                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }
            string? dentistId = await patientService.GetDentistIdAsync();

            if (dentistId == null)
            {
                return BadRequest("Dentist user is not configured.");
            }

            Appointment currentAppointment = new Appointment
            {
                PatientId = userManager.GetUserId(User)!,
                DentistId = dentistId,
                Date = appointmentDate,
                PatientPhoneNumber = createModel.PatientPhoneNumber,
                ManipulationTypeId = createModel.ManipulationTypeId,
                Note = createModel.Note

            };

            await dbContext.Appointments.AddAsync(currentAppointment);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            Appointment? appointmentToDelete = await dbContext
                .Appointments
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.AppointmentId.ToString().ToLower() == id.ToLower());

            if (appointmentToDelete == null)
            {
                return NotFound();
            }
            appointmentToDelete.IsDeleted = true;

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Appointment? appointmentToEdit = await dbContext
                .Appointments
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.AppointmentId == id);

            if (appointmentToEdit == null)
            {
                return NotFound();
            }

            AppointmentCreateViewModel editViewModel = new AppointmentCreateViewModel
            {
                AppointmentId = appointmentToEdit.AppointmentId.ToString(),
                AppointmentDate = appointmentToEdit.Date.Date,
                AppointmentTime = appointmentToEdit.Date.TimeOfDay,
                PatientPhoneNumber = appointmentToEdit.PatientPhoneNumber,
                ManipulationTypeId = appointmentToEdit.ManipulationTypeId,
                Note = appointmentToEdit.Note,


            };

            await PopulateManipulationTypesAsync(editViewModel);
            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentCreateViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }
            DateTime appointmentDate = editViewModel.AppointmentDate.Date + editViewModel.AppointmentTime;

            if (await this.dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(a => a.Date == appointmentDate 
                && a.IsDeleted == false 
                && editViewModel.AppointmentId!.ToLower() != a.AppointmentId.ToString().ToLower()))
                
            {
                ModelState
                    .AddModelError(nameof(editViewModel.AppointmentDate), "Duplicate applointment hour");
                await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }

            if (!ValidateManipulationId(dbContext, editViewModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.ManipulationTypeId), "The selected manipulation is incorrect");
                await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }

            if (appointmentDate < DateTime.Now)
            {
                ModelState
                   .AddModelError(nameof(editViewModel.AppointmentDate), "You should not set an appintment in the past");
                await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }

            Appointment? appointmentToEdit = await dbContext
                .Appointments
                .SingleOrDefaultAsync(m => m.IsDeleted == false && m.AppointmentId.ToString().ToLower() == editViewModel.AppointmentId!.ToLower());

            if (appointmentToEdit == null)
            {
                return NotFound();
            }

            appointmentToEdit.Date = appointmentDate;
            appointmentToEdit.PatientPhoneNumber = editViewModel.PatientPhoneNumber;
            appointmentToEdit.ManipulationTypeId = editViewModel.ManipulationTypeId;
            appointmentToEdit.Note = editViewModel.Note;
           
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateManipulationTypesAsync(AppointmentCreateViewModel createViewModel)
        {
            IEnumerable<LookupItem> manipulationTypes = await manipulationService.GetManipulationTypesAsync();

            createViewModel.ManipulationTypes = manipulationTypes
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                }); 
        }
        private bool ValidateManipulationId(DentistAppDbContext dbContext, Guid currentManipulation)
        {
            bool isManipulationValid = dbContext.ManipulationTypes
                .Any(m=>m.ManipulationId == currentManipulation);
            return isManipulationValid;
        }              
    }
}
