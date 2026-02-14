namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using static DentistApp.GCommon.GlobalCommon;
    using static DentistApp.GCommon.ValidationMessages;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ProcedureViewModels;

    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    
    public class ProcedureService:IProcedureService
    {
        private readonly DentistAppDbContext dbContext;
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        public ProcedureService(DentistAppDbContext dbContext, IManipulationService manipulationService, IPatientService patientService)
        {
            this.dbContext = dbContext;
            this.manipulationService = manipulationService;
            this.patientService = patientService;
        }

        public async Task CreateProcedureAsync(ProcedureCreateViewModel procedureToCreate, string dentistId)
        { 
            bool isManipulationCorrect = await manipulationService.ValidateManipulationTypesAsync(procedureToCreate.ManipulationTypeId);
            if (!isManipulationCorrect)
            {
                throw new Exception(ManipulationNotCorrectValidationMessage);
            }
            if (procedureToCreate.ProcedureDate > DateTime.Today)
            {
                throw new Exception(ProcedureCannotBeInTheFutureValidationMessage);
            }
            
            if (!await patientService.IsUserDentistByIdAsync(dentistId))
            {
                throw new Exception(ProcedureCreatorIsNotDentistValidationMessage);
            }
            if (!await patientService.IsUserInDbByIdAsync(procedureToCreate.PatientId))
            {
                throw new Exception(ProcedureCreatorNotInDatabaseValidationMessage);
            }

            Procedure currentProcedure = new Procedure
            {
                PatientId = procedureToCreate.PatientId.ToString(),
                DentistId = dentistId,
                Date = procedureToCreate.ProcedureDate,
                PatientPhoneNumber = procedureToCreate.PatientPhoneNumber,
                ManipulationTypeId = procedureToCreate.ManipulationTypeId,
                Note = procedureToCreate.DentistNote
            };

            await dbContext.Procedures.AddAsync(currentProcedure);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteProcedureByIdAsync(Guid procedureId)
        {
            Procedure? procedureToDelete = await this.GetProcedureByIdAsync(procedureId);
            if (procedureToDelete == null)
            {
                throw new Exception(ProcedureCannotBeFoundValidationMessage);
            }

            procedureToDelete.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }     

        public async Task <IEnumerable<ProcedureViewViewModel>> GetAllProceduresViewModelsAsync(string userId)
        {
            IEnumerable<ProcedureViewViewModel> procedures = await dbContext
               .Procedures
               .AsNoTracking()
               .Include(p => p.ManipulationType)
               .Include(p => p.Dentist)
               .Include(p => p.Patient)
               .Where(p => p.DentistId == userId || p.PatientId == userId)
               .OrderBy(p => p.Date)
               .Select(p => new ProcedureViewViewModel
               {
                   ProcedureId = p.ProcedureId.ToString(),
                   PatientProcedureName = $"{p.Patient.FirstName} {p.Patient.LastName}",
                   DentistProcedureName = $"{p.Dentist.FirstName} {p.Dentist.LastName}",
                   ProcedureDate = p.Date.ToString(ApplicationDateTimeFormat, CultureInfo.InvariantCulture),
                   PatientProcedurePhoneNumber = p.PatientPhoneNumber,
                   ManipulationName = p.ManipulationType.Name,
                   ProcedureDentistNote = p.Note
               }).ToArrayAsync();
            return procedures;
        }

        public async Task <ProcedureCreateViewModel> GetCreateViewModelAsync()
        {
            IEnumerable<DropDown> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            ProcedureCreateViewModel createModel = new ProcedureCreateViewModel();
            createModel.ProcedureDate = DateTime.Today;
            createModel.ManipulationTypes =await manipulationService.GetManipulationTypesAsync();
            createModel.PatientsNames = await patientService.GetPatientsAsync();
            return createModel;
        }

        public async Task <Procedure?> GetProcedureByIdAsync(Guid procedureId)
        {
            return await dbContext
                .Procedures
                .SingleOrDefaultAsync(a => a.IsDeleted == false && a.ProcedureId == procedureId);
        }

        public async Task <ProcedureCreateViewModel> LoadProcedureEditViewModelByIdAsync(Guid procedureId)
        {
            Procedure? procedureToEdit = await this.GetProcedureByIdAsync(procedureId);
            if (procedureToEdit == null)
            {
                throw new Exception(ProcedureCannotBeFoundValidationMessage);
            }

            ProcedureCreateViewModel editViewModel = new ProcedureCreateViewModel
            {
                ProcedureId = procedureToEdit.ProcedureId,
                ProcedureDate = procedureToEdit.Date,
                PatientId = procedureToEdit.PatientId,
                PatientPhoneNumber = procedureToEdit.PatientPhoneNumber,
                ManipulationTypes = await manipulationService.GetManipulationTypesAsync(),
                PatientsNames = await patientService.GetPatientsAsync(),
                ManipulationTypeId = procedureToEdit.ManipulationTypeId,
                DentistNote = procedureToEdit.Note,
            };
            return editViewModel;
        }

        public async Task EditProcedureAsync(ProcedureCreateViewModel procedureToEdit, string dentistId)
        {
            bool isManipulationCorrect = await manipulationService.ValidateManipulationTypesAsync(procedureToEdit.ManipulationTypeId);

            if (!isManipulationCorrect)
            {
                throw new Exception(ManipulationNotCorrectValidationMessage);
            }

            if (procedureToEdit.ProcedureDate > DateTime.Today)
            {
                throw new Exception(ProcedureCannotBeInTheFutureValidationMessage);
            }
                        
            if (!await patientService.IsUserInDbByIdAsync(dentistId))
            {
                throw new Exception(ProcedureDentistNotInDatabaseValidationMessage);
            }

            if (!await patientService.IsUserInDbByIdAsync(procedureToEdit.PatientId))
            {
                throw new Exception(ProcedureCreatorNotInDatabaseValidationMessage);
            }

            if (!await patientService.IsUserDentistByIdAsync(dentistId))
            {
                throw new Exception(ProcedureCreatorIsNotDentistValidationMessage);
            }
            Procedure? editProcedure =await this.GetProcedureByIdAsync(procedureToEdit.ProcedureId!.Value);
            if (editProcedure == null)
            {
                throw new Exception(ProcedureCannotBeFoundValidationMessage);
            }

            editProcedure.Date = procedureToEdit.ProcedureDate;
            editProcedure.PatientPhoneNumber = procedureToEdit.PatientPhoneNumber;
            editProcedure.ManipulationTypeId = procedureToEdit.ManipulationTypeId;
            editProcedure.Note = procedureToEdit.DentistNote;
            editProcedure.PatientId = procedureToEdit.PatientId;
            editProcedure.DentistId = dentistId;

            await dbContext.SaveChangesAsync();
        }

        public async Task <bool> IsProcedureDateInTheFuture(DateTime procedureDate)
        {
            return procedureDate > DateTime.Today;
        }

        public async Task<bool> IsProcedureValid(Guid procedureId)
        {
            return await dbContext
                .Procedures
                .AnyAsync(a => a.IsDeleted == false && 
                a.ProcedureId == procedureId);
        }
    }
}
