namespace DentistApp.Web.Controllers
{
    using DentistApp.Data.Models;
    using static DentistApp.GCommon.Roles;
    using static DentistApp.GCommon.ControllersOutputMessages;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.ProcedureViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;

    [Authorize]
    public class ProcedureController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        private readonly IProcedureService procedureService;

        public ProcedureController(UserManager<ApplicationUser> userManager, IManipulationService manipulationService, IPatientService patientService, IProcedureService procedureService)
        {
            this.userManager = userManager;
            this.manipulationService = manipulationService;
            this.patientService = patientService;
            this.procedureService = procedureService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string currentUserId = userManager.GetUserId(User)!;
            IEnumerable<ProcedureViewViewModel> procedures = await procedureService
                .GetAllProceduresViewModelsAsync(currentUserId);
            return View(procedures);
        }

        [HttpGet]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Create()
        {
            ProcedureCreateViewModel createModel =await procedureService
                .GetCreateViewModelAsync();
            return View(createModel);
        }
        
        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Create(ProcedureCreateViewModel createModel)
        {
            createModel.ManipulationTypes =await manipulationService
                .GetManipulationTypesAsync();
            createModel.PatientsNames = await patientService
                .GetPatientsAsync();
            if (!ModelState.IsValid)
            {
                return View(createModel);
            }

            if (!await manipulationService
                .ValidateManipulationTypesAsync(createModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(createModel.ManipulationTypeId), ManipulationIsIncorrect);
                return View(createModel);
            }

            if (!await patientService.IsUserInDbByIdAsync(createModel.PatientId))
            {
                ModelState
                    .AddModelError(nameof(createModel.PatientId), ProcedurePatientIsIncorrect);
                return View(createModel);
            }

            if (await procedureService.IsProcedureDateInTheFuture(createModel.ProcedureDate.Date))
            {
                ModelState
                   .AddModelError(nameof(createModel.ProcedureDate), ProcedureSetInTheFuture);
                return View(createModel);
            }
            string dentistId = userManager.GetUserId(User)!;
            if (!await patientService.IsUserInDbByIdAsync(dentistId))
            {
                ModelState
                   .AddModelError(string.Empty, ProcedureUserIsNotInDatabase);
            }
            try
            {
                await procedureService
                    .CreateProcedureAsync(createModel, dentistId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, ProcedureCreationError);
                return View(createModel);
            }   
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await procedureService.IsProcedureValid(id))
            {
                return NotFound();
            }

            try
            {
                await procedureService.DeleteProcedureByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }  
        }

        [HttpGet]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await procedureService.IsProcedureValid(id))
            {
                return NotFound();
            }

            try
            {
                ProcedureCreateViewModel editViewModel = await procedureService.LoadProcedureEditViewModelByIdAsync(id);
                return View(editViewModel);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Edit(ProcedureCreateViewModel editViewModel)
        {
            editViewModel.ManipulationTypes = await manipulationService
                .GetManipulationTypesAsync();
            editViewModel.PatientsNames = await patientService
                .GetPatientsAsync();
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }

            if (!await manipulationService
                .ValidateManipulationTypesAsync(editViewModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.ManipulationTypeId), ManipulationIsIncorrect);
                return View(editViewModel);
            }

            if (!await patientService.IsUserInDbByIdAsync(editViewModel.PatientId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.PatientId), ProcedurePatientIsIncorrect);
                return View(editViewModel);
            }

            if (await procedureService.IsProcedureDateInTheFuture(editViewModel.ProcedureDate))
            {
                ModelState
                   .AddModelError(nameof(editViewModel.ProcedureDate), ProcedureSetInTheFuture);
                return View(editViewModel);
            }
            if (!editViewModel.ProcedureId.HasValue)
            {
                return NotFound();
            }
                        
            if (!await procedureService.IsProcedureValid(editViewModel.ProcedureId.Value))
            {
                return NotFound();
            }
            string dentistId = userManager.GetUserId(User)!;
            if (!await patientService.IsUserInDbByIdAsync(dentistId))
            {
                ModelState
                   .AddModelError(string.Empty, ProcedureUserIsNotInDatabase);
            }

            try
            {
                await procedureService.EditProcedureAsync(editViewModel, dentistId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, ProcedureCreationError);
                return View(editViewModel);
            }
        }   
    }
}
