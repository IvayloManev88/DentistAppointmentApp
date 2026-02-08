namespace DentistApp.Web.Controllers
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.AppointmentViewModels;

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
        private readonly IAppointmentService appointmentService;
        
        public AppointmentController(UserManager<ApplicationUser> userManager, DentistAppDbContext dbContext, IManipulationService manipulationService, IPatientService patientService, IAppointmentService appointmentService)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.manipulationService = manipulationService;
            this.patientService = patientService;
            this.appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<AppointmentViewAppointmentViewModel> appointments =await appointmentService.GetAllAppotinmentsViewModelsAsync();
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            AppointmentCreateViewModel createModel = await appointmentService.CreateViewModelAsync();
            return View(createModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateViewModel createModel)
        {
            createModel.ManipulationTypes = await manipulationService.GetManipulationTypesAsync();
            if (!ModelState.IsValid)
            {
                return View(createModel);
            }

            DateTime appointmentDateTime = createModel.AppointmentDate.Date + createModel.AppointmentTime;
            
            if (!await manipulationService.ValidateManipulationTypesAsync(createModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(createModel.ManipulationTypeId), "The selected manipulation is incorrect");
                
                return View(createModel);
            }          

            if (await appointmentService.AppointmentDuplicateDateAndTimeAsync(appointmentDateTime))
            {
                ModelState
                    .AddModelError(nameof(createModel.AppointmentDate), "The selected combination Date/Time is already taken. Please try different Date/Time");
                
                return View(createModel);
            }

            if (appointmentDateTime < DateTime.Today)
            {
                ModelState
                   .AddModelError(nameof(createModel.AppointmentDate), "You should not set an appintment in the past");
                return View(createModel);
            }
            string? dentistId = await patientService.GetDentistIdAsync();

            if (dentistId == null)
            {
                return BadRequest("Dentist user is not configured.");
            }
            string patientId = userManager.GetUserId(User)!;
            try
            {
                await appointmentService.CreateAppointmentAsync(createModel, appointmentDateTime, patientId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating Appointment.Please try again!");
                return View(createModel);
            }
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
            string currentUserId = userManager.GetUserId(User)!;
            Appointment? appointmentToEdit = await dbContext
                .Appointments
                .Where(a=>a.PatientId== currentUserId || a.DentistId== currentUserId)
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

        //    await PopulateManipulationTypesAsync(editViewModel);
            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentCreateViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }
            string currentUserId = userManager.GetUserId(User)!;
            Appointment? appointmentToEdit = await dbContext
                .Appointments
                .Where(a => a.PatientId == currentUserId || a.DentistId == currentUserId)
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.AppointmentId.ToString() == editViewModel.AppointmentId);

            if (appointmentToEdit == null)
            {
                return NotFound();
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
                //await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }

           // if (!ValidateManipulationId(dbContext, editViewModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.ManipulationTypeId), "The selected manipulation is incorrect");
             //   await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }

            if (appointmentDate < DateTime.Now)
            {
                ModelState
                   .AddModelError(nameof(editViewModel.AppointmentDate), "You should not set an appintment in the past");
               // await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }
                   
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

       
        

       
    }
}
