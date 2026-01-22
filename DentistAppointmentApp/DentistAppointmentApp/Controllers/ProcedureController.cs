namespace DentistApp.Web.Controllers
{

    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.Services.Core.Models;
    using DentistApp.Web.ViewModels.AppointmentViewModels;
    using DentistApp.Web.ViewModels.ProcedureViewModels;
    using DentistAppointmentApp.Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;

    using static GCommon.ProcedureConstants;
    public class ProcedureController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DentistAppDbContext dbContext;
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;


        public ProcedureController(UserManager<ApplicationUser> userManager, DentistAppDbContext dbContext, IManipulationService manipulationService, IPatientService patientService)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.manipulationService = manipulationService;
            this.patientService = patientService;

        }
        
        public async Task<IActionResult> Index()
        {
            string currentUserId = userManager.GetUserId(User)!;
            ProcedureViewViewModel[] procedures = await dbContext
                .Procedures
                .AsNoTracking()
                .Include(p => p.ManipulationType)
                .Include(p=>p.Dentist)
                .Include(p=>p.Patient)
                .Where(p=>p.DentistId== currentUserId ||
                p.PatientId == currentUserId)
                .OrderBy(p => p.Date)
                .Select(p => new ProcedureViewViewModel
                {
                    ProcedureId = p.ProcedureId.ToString(),
                    PatientProcedureName = $"{p.Patient.FirstName} {p.Patient.LastName}",
                    DentistProcedureName = $"{p.Dentist.FirstName} {p.Dentist.LastName}",
                    ProcedureDate = p.Date.ToString("dd.MM.yyyy"),
                    PatientProcedurePhoneNumber = p.PatientPhoneNumber,
                    ManipulationName = p.ManipulationType.Name,
                    ProcedureDentistNote = p.Note

                }).ToArrayAsync();

            return View(procedures);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<LookupItem> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            ProcedureCreateViewModel createModel = new ProcedureCreateViewModel();
            await PopulateManipulationTypesAsync(createModel);
            await PopulatePatientsAsync(createModel);
            return View(createModel);
        }
        /*
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateViewModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createModel);
            }

            DateTime appointmentDate = createModel.AppointmentDate.Date + createModel.AppointmentTime;
            if (await this.dbContext.Appointments.AsNoTracking().AnyAsync(a => a.Date == appointmentDate && a.IsDeleted == false))
            {
                ModelState
                    .AddModelError(nameof(createModel.AppointmentDate), "The selected combination Date/Time is not available. Please try different Date/Time");

                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }

            if (appointmentDate < DateTime.Now)
            {
                ModelState
                   .AddModelError(nameof(createModel.AppointmentDate), "You should not set an appintment in the past");
                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }
            Appointment currentAppointment = new Appointment
            {
                PatientId = userManager.GetUserId(User)!,
                DentistId = DentistId,
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
        public async Task<IActionResult> Edit(string id)
        {
            Appointment? appointmentToEdit = await dbContext
                .Appointments
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.AppointmentId.ToString().ToLower() == id.ToLower());

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
        */
        private async Task PopulateManipulationTypesAsync(ProcedureCreateViewModel createViewModel)
        {
            IEnumerable<LookupItem> manipulationTypes = await manipulationService.GetManipulationTypesAsync();

            createViewModel.ManipulationTypes = manipulationTypes
                .Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                });

        }

        private async Task PopulatePatientsAsync(ProcedureCreateViewModel createViewModel)
        {
            IEnumerable<LookupItem> patientNames = await patientService.GetPatientsAsync();

            createViewModel.PatientsNames = patientNames
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                });

        }
        
    }
}
