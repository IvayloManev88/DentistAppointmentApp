namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.AppointmentsScheduleViewModels;
    using DentistApp.ViewModels.AppointmentViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using static DentistApp.GCommon.ControllersOutputMessages;

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
            string currentUserId = userManager.GetUserId(User)!;
            IEnumerable<AppointmentViewAppointmentViewModel> appointments = await appointmentService.GetAllAppotinmentsViewModelsAsync(currentUserId);
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? selectedDate, string? selectedTime)
        {
            
            AppointmentCreateViewModel createModel = await appointmentService.CreateViewModelAsync(selectedDate, selectedTime);
            
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
                    .AddModelError(nameof(createModel.ManipulationTypeId), ManipulationIsIncorrect);

                return View(createModel);
            }

            if (await appointmentService.AppointmentDuplicateDateAndTimeAsync(appointmentDateTime))
            {
                ModelState
                    .AddModelError(nameof(createModel.AppointmentDate), AppointmentDateTimeTaken);

                return View(createModel);
            }

            if (await appointmentService.IsAppointmentNotInFuture(appointmentDateTime))
            {
                ModelState
                   .AddModelError(nameof(createModel.AppointmentDate), AppointmentSetInThePast);
                return View(createModel);
            }
            
            if (await patientService.GetDentistIdAsync()==null)
            {
                return BadRequest(DentistUserNotConfigured);
            }
            string patientId = userManager.GetUserId(User)!;
            try
            {
                await appointmentService.CreateAppointmentAsync(createModel, patientId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, AppointmentCreationError);
                return View(createModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            string currentUserId = userManager.GetUserId(User)!;
           
            if (!await appointmentService.CanAppointmentBeManipulatedByUserIdAsync(id, currentUserId))
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
            
            if (!await appointmentService.CanAppointmentBeManipulatedByUserIdAsync(id, currentUserId))
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
            
            if (!await appointmentService.CanAppointmentBeManipulatedByUserIdAsync(editViewModel.AppointmentId.Value, currentUserId))
            {
                return NotFound();
            }
            DateTime appointmentDate = editViewModel.AppointmentDate.Date + editViewModel.AppointmentTime;

            if (await appointmentService.AppointmentDuplicateDateAndTimeAsync(appointmentDate, editViewModel.AppointmentId))

            {
                ModelState
                    .AddModelError(nameof(editViewModel.AppointmentDate), AppointmentDateTimeTaken);
                return View(editViewModel);
            }

            if (!await manipulationService.ValidateManipulationTypesAsync(editViewModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.ManipulationTypeId), ManipulationIsIncorrect);
                return View(editViewModel);
            }

            if (await appointmentService.IsAppointmentNotInFuture(appointmentDate))
            {
                ModelState
                   .AddModelError(nameof(editViewModel.AppointmentDate), AppointmentSetInThePast);
                return View(editViewModel);
            }

            try
            {
                await appointmentService.EditAppointmentAsync(editViewModel);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, AppointmentCreationError);
                return View(editViewModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> WeeklySchedulePartial(DateTime? weekStartDate)
        {
            DateTime selectedDate = weekStartDate ?? DateTime.Today;

            WeeklyScheduleViewModel model =
                await appointmentService.GetWeeklyScheduleAsync(selectedDate);

            return PartialView("_WeeklySchedulePartial", model);
        }
    }
}
