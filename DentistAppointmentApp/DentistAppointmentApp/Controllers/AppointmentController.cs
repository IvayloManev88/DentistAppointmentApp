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
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        private readonly IAppointmentService appointmentService;

        public AppointmentController(UserManager<ApplicationUser> userManager, IManipulationService manipulationService, IPatientService patientService, IAppointmentService appointmentService)
        {
            this.userManager = userManager;
            this.manipulationService = manipulationService;
            this.patientService = patientService;
            this.appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<AppointmentViewAppointmentViewModel> appointments = await appointmentService.GetAllAppotinmentsViewModelsAsync();
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
        public async Task<IActionResult> Delete(Guid id)
        {
            Appointment? appointmentToDelete = await appointmentService.GetAppointmentByIdAsync(id);

            if (appointmentToDelete == null)
            {
                return NotFound();
            }
            try
            {
                await appointmentService.DeleteAppointmentByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            /*An appointment should be edited only by the user created the appointment or the dentist*/
            string currentUserId = userManager.GetUserId(User)!;
            Appointment? appointmentToEdit = await appointmentService.GetAppointmentToEditByUserIdAsync(id, currentUserId);

            if (appointmentToEdit == null)
            {
                return NotFound();
            }
            try
            {
                AppointmentCreateViewModel editViewModel = await appointmentService.LoadEditViewModelByIdAsync(id);
                return View(editViewModel);
            }
            catch
            {
                return NotFound();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentCreateViewModel editViewModel)
        {
            editViewModel.ManipulationTypes = await manipulationService.GetManipulationTypesAsync();
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }
            string currentUserId = userManager.GetUserId(User)!;
            if (!editViewModel.AppointmentId.HasValue)
            {
                return NotFound();
            }
            Appointment? appointmentToEdit = await appointmentService.GetAppointmentToEditByUserIdAsync(editViewModel.AppointmentId.Value, currentUserId);

            if (appointmentToEdit == null)
            {
                return NotFound();
            }
            DateTime appointmentDate = editViewModel.AppointmentDate.Date + editViewModel.AppointmentTime;

            if (await appointmentService.AppointmentDuplicateDateAndTimeAsync(appointmentDate, editViewModel.AppointmentId))

            {
                ModelState
                    .AddModelError(nameof(editViewModel.AppointmentDate), "Duplicate appointment hour");
                return View(editViewModel);
            }

            if (!await manipulationService.ValidateManipulationTypesAsync(editViewModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.ManipulationTypeId), "The selected manipulation is incorrect");
                return View(editViewModel);
            }

            if (appointmentDate < DateTime.Now)
            {
                ModelState
                   .AddModelError(nameof(editViewModel.AppointmentDate), "You should not set an appointment in the past");
                return View(editViewModel);
            }

            try
            {
                await appointmentService.EditAppointmentAsync(editViewModel, appointmentToEdit, appointmentDate);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while editing an Appointment.Please try again!");
                return View(editViewModel);
            }

        }
    }
}
