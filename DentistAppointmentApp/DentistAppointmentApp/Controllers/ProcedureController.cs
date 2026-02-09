namespace DentistApp.Web.Controllers
{

    using DentistApp.Data;
    using DentistApp.Data.Models;

    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels.ProcedureViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;

    using static DentistApp.GCommon.Roles;

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

            DateTime procedureDate = createModel.ProcedureDate.Date;
            ManipulationType? currentManipulation = await manipulationService.GetManipulationByIdAsync(createModel.ManipulationTypeId);
            if (!await manipulationService
                .ValidateManipulationTypesAsync(createModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(createModel.ManipulationTypeId), "The selected manipulation is incorrect");
                return View(createModel);
            }

            if (!await patientService.IsUserInDbByIdAsync(createModel.PatientId))
            {
                ModelState
                    .AddModelError(nameof(createModel.PatientId), "The selected patient is incorrect");
                return View(createModel);
            }

            if (procedureDate > DateTime.Today)
            {
                ModelState
                   .AddModelError(nameof(createModel.ProcedureDate), "You should not set procedure that is done in the future");
                return View(createModel);
            }
            string dentistId = userManager.GetUserId(User)!;
            try
            {
                await procedureService
                    .CreateProcedureAsync(createModel, dentistId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating Procedure.Please try again!");
                return View(createModel);
            }   
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Procedure? procedureToDelete = await procedureService
                .GetProcedureByIdAsync(id);

            if (procedureToDelete == null)
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
            Procedure? procedureToEdit = await procedureService
                .GetProcedureByIdAsync(id);

            if (procedureToEdit == null)
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
                    .AddModelError(nameof(editViewModel.ManipulationTypeId), "The selected manipulation is incorrect");
                return View(editViewModel);
            }

            if (!await patientService.IsUserInDbByIdAsync(editViewModel.PatientId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.PatientId), "The selected patient is incorrect");
                return View(editViewModel);
            }

            if (editViewModel.ProcedureDate > DateTime.Now)
            {
                ModelState
                   .AddModelError(nameof(editViewModel.ProcedureDate), "You should not set procedure that is done in the future");
                return View(editViewModel);
            }
            if (!editViewModel.ProcedureId.HasValue)
            {
                return NotFound();
            }
            Procedure? procedureToEdit = await procedureService.GetProcedureByIdAsync(editViewModel.ProcedureId.Value);
            
            if (procedureToEdit == null)
            {
                return NotFound();
            }
            string dentistId = userManager.GetUserId(User)!;

            try
            {
                await procedureService.EditProcedureAsync(editViewModel, procedureToEdit);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while editing a Procedure.Please try again!");
                return View(editViewModel);
            }
        }   
    }
}
