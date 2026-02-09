namespace DentistApp.Web.Controllers
{

    using DentistApp.Data;
    using DentistApp.Data.Models;

    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ProcedureViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;

    using static DentistApp.GCommon.Roles;
    using static GCommon.GlobalCommon;

    [Authorize]
    public class ProcedureController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DentistAppDbContext dbContext;
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        private readonly IProcedureService procedureService;

        public ProcedureController(UserManager<ApplicationUser> userManager, DentistAppDbContext dbContext, IManipulationService manipulationService, IPatientService patientService, IProcedureService procedureService)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.manipulationService = manipulationService;
            this.patientService = patientService;
            this.procedureService = procedureService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string currentUserId = userManager.GetUserId(User)!;
            IEnumerable<ProcedureViewViewModel> procedures = await procedureService.GetAllProceduresViewModelsAsync(currentUserId);
            return View(procedures);
        }

        [HttpGet]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Create()
        {
            IEnumerable<DropDown> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            ProcedureCreateViewModel createModel = new ProcedureCreateViewModel();
            createModel.ProcedureDate = DateTime.Today;
            await PopulateManipulationTypesAsync(createModel);
            await PopulatePatientsAsync(createModel);
            return View(createModel);
        }
        
        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Create(ProcedureCreateViewModel createModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateManipulationTypesAsync(createModel);
                await PopulatePatientsAsync(createModel);
                return View(createModel);
            }

            DateTime procedureDate = createModel.ProcedureDate.Date;
            if (!ValidateManipulationId(dbContext, createModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(createModel.ManipulationTypeId), "The selected manipulation is incorrect");
                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }

            if (!ValidatePatientId(dbContext, createModel.PatientId))
            {
                ModelState
                    .AddModelError(nameof(createModel.PatientId), "The selected patient is incorrect");
                await PopulateManipulationTypesAsync(createModel);
                return View(createModel);
            }

            if (procedureDate > DateTime.Today)
            {
                ModelState
                   .AddModelError(nameof(createModel.ProcedureDate), "You should not set procedure that is done in the future");
                await PopulateManipulationTypesAsync(createModel);
                await PopulatePatientsAsync(createModel);
                return View(createModel);
            }
            Procedure currentProcedure = new Procedure
            {
                PatientId = createModel.PatientId.ToString(),
                DentistId = userManager.GetUserId(User)!,
                Date = procedureDate,
                PatientPhoneNumber = createModel.PatientPhoneNumber,
                ManipulationTypeId = createModel.ManipulationTypeId,
                Note = createModel.DentistNote
            };

            await dbContext.Procedures.AddAsync(currentProcedure);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Procedure? procedureToDelete = await dbContext
                .Procedures
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.ProcedureId== id);

            if (procedureToDelete == null)
            {
                return NotFound();
            }

            procedureToDelete.IsDeleted = true;

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Edit(Guid id)
        {
            Procedure? procedureToEdit = await dbContext
                .Procedures
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.ProcedureId == id);

            if (procedureToEdit == null)
            {
                return NotFound();
            }

            ProcedureCreateViewModel editViewModel = new ProcedureCreateViewModel
            {
                ProcedureId = procedureToEdit.ProcedureId.ToString(),
                ProcedureDate = procedureToEdit.Date,
                PatientId = procedureToEdit.PatientId,
                PatientPhoneNumber = procedureToEdit.PatientPhoneNumber,
                ManipulationTypeId = procedureToEdit.ManipulationTypeId,
                DentistNote = procedureToEdit.Note,


            };

            await PopulateManipulationTypesAsync(editViewModel);
            await PopulatePatientsAsync(editViewModel);
            return View(editViewModel);
        }

        [HttpPost]
        [Authorize(Roles = DentistRoleName)]
        public async Task<IActionResult> Edit(ProcedureCreateViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateManipulationTypesAsync(editViewModel);
                await PopulatePatientsAsync(editViewModel);
                return View(editViewModel);
            }

            if (!ValidateManipulationId(dbContext, editViewModel.ManipulationTypeId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.ManipulationTypeId), "The selected manipulation is incorrect");
                await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }

            if (!ValidatePatientId(dbContext, editViewModel.PatientId))
            {
                ModelState
                    .AddModelError(nameof(editViewModel.PatientId), "The selected patient is incorrect");
                await PopulateManipulationTypesAsync(editViewModel);
                return View(editViewModel);
            }

            if (editViewModel.ProcedureDate > DateTime.Now)
            {
                ModelState
                   .AddModelError(nameof(editViewModel.ProcedureDate), "You should not set procedure that is done in the future");
                await PopulateManipulationTypesAsync(editViewModel);
                await PopulatePatientsAsync(editViewModel);
                return View(editViewModel);
            }

            Procedure? procedureToEdit = await dbContext
                .Procedures
                .SingleOrDefaultAsync(m => m.IsDeleted == false && m.ProcedureId.ToString()== editViewModel.ProcedureId!);

            if (procedureToEdit == null)
            {
                return NotFound();
            }

            procedureToEdit.Date = editViewModel.ProcedureDate;
            procedureToEdit.PatientPhoneNumber = editViewModel.PatientPhoneNumber;
            procedureToEdit.ManipulationTypeId = editViewModel.ManipulationTypeId;
            procedureToEdit.Note = editViewModel.DentistNote;
            procedureToEdit.PatientId = editViewModel.PatientId;
            procedureToEdit.DentistId = userManager.GetUserId(User)!;

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private async Task PopulateManipulationTypesAsync(ProcedureCreateViewModel createViewModel)
        {
            IEnumerable<DropDown> manipulationTypes = await manipulationService.GetManipulationTypesAsync();

            createViewModel.ManipulationTypes = manipulationTypes
                .Select(mt => new DropDown
                {
                    Id = mt.Id,
                    Name = mt.Name
                });
        }

        private async Task PopulatePatientsAsync(ProcedureCreateViewModel createViewModel)
        {
            IEnumerable<DropDown> patientNames = await patientService.GetPatientsAsync();

            createViewModel.PatientsNames = patientNames
                .Select(p => new DropDown
                {
                    Id = p.Id,
                    Name = p.Name
                });

        }

        private bool ValidateManipulationId(DentistAppDbContext dbContext, Guid currentProcedureManipulationId)
        {
            bool isManipulationValid = dbContext.ManipulationTypes
                .Any(m => m.ManipulationId == currentProcedureManipulationId);

            return isManipulationValid;
        }

        private bool ValidatePatientId(DentistAppDbContext dbContext, string currentProcedurePatientId)
        {
            bool isPatientValid = dbContext.Users
                .Any(u=>u.Id == currentProcedurePatientId.ToString());

            return isPatientValid;
        }

    }
}
