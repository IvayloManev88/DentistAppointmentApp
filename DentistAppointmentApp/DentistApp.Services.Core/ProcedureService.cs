namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ProcedureViewModels;

    using Microsoft.EntityFrameworkCore;

    using System.Globalization;

    using static DentistApp.GCommon.GlobalCommon;
    using static DentistApp.GCommon.ValidationMessages;

    public class ProcedureService : IProcedureService
    {
        private readonly IManipulationService manipulationService;
        private readonly IPatientService patientService;
        private readonly IProcedureRepository procedureRepository;
        private readonly IDateTimeService dateTimeService;
        public ProcedureService(IManipulationService manipulationService, IPatientService patientService, IProcedureRepository procedureRepository, IDateTimeService dateTimeService)
        {
            this.manipulationService = manipulationService;
            this.patientService = patientService;
            this.procedureRepository = procedureRepository;
            this.dateTimeService = dateTimeService;
        }
        public async Task<Procedure?> GetProcedureByIdAsync(Guid procedureId)
        {
            return await procedureRepository
                .GetProcedureByIdAsync(procedureId);
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

            await procedureRepository
                .AddAsync(currentProcedure);
            await procedureRepository
                .SaveChangesAsync();
        }

        public async Task DeleteProcedureByIdAsync(Guid procedureId)
        {
            Procedure? procedureToDelete = await this.GetProcedureByIdAsync(procedureId);
            if (procedureToDelete == null)
            {
                throw new Exception(ProcedureCannotBeFoundValidationMessage);
            }

            await procedureRepository
                .SoftDeleteAppointmentAsync(procedureToDelete);
        }

        public async Task<ProcedurePaginationViewModel> GetAllProceduresViewModelsAsync(string userId, string? searchQuery=null, int page = 1)
        {
            IQueryable<Procedure> queryProcedures = await procedureRepository
                .GetQueryableProceduresAsync(userId);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string normalizedQuery = searchQuery.ToLower().Trim();
                queryProcedures = queryProcedures.Where(p=>
                (p.Patient.FirstName + " " + p.Patient.LastName).ToLower().Contains(normalizedQuery)||
                p.ManipulationType.Name.ToLower().Contains(normalizedQuery));
            }
            int pageSize = ItemsPerPage;
            int totalItemsQueryCount =await queryProcedures.CountAsync();

            page = Math.Max(1, page);
           
            IEnumerable<ProcedureViewViewModel> procedures = await queryProcedures
                .OrderByDescending(p => p.Date)
                .Skip((page-1)*pageSize)
                .Take(pageSize)
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

            ProcedurePaginationViewModel resultViewModel = new ProcedurePaginationViewModel
            {
                Procedures = procedures,
                SearchQuery = searchQuery,
                CurrentPage = page,
                ProceduresPerPage = pageSize,
                TotalItemsCount = totalItemsQueryCount
            };

            return resultViewModel;
        }

        public async Task<ProcedureCreateViewModel> GetCreateViewModelAsync()
        {
            IEnumerable<DropDown> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            ProcedureCreateViewModel createModel = new ProcedureCreateViewModel();
            createModel.ProcedureDate = DateTime.Today;
            createModel.ManipulationTypes = await manipulationService.GetManipulationTypesAsync();
            createModel.PatientsNames = await patientService.GetPatientsAsync();
            return createModel;
        }

        public async Task<ProcedureCreateViewModel> LoadProcedureEditViewModelByIdAsync(Guid procedureId)
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
            Procedure? editProcedure = await this.GetProcedureByIdAsync(procedureToEdit.ProcedureId!.Value);
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

            await procedureRepository.SaveChangesAsync();
        }

        public async Task<bool> IsProcedureDateInTheFuture(DateTime procedureDate)
        {
            return procedureDate > dateTimeService.Today();
        }

        public async Task<bool> IsProcedureValid(Guid procedureId)
        {
            return await procedureRepository
                .IsProcedureValidAsync(procedureId);
        }

        public async Task<Guid?> GetLatestProcedureByUserIdAsync(string userId)
        {
           return await procedureRepository
                .GetLatestProcedureByUserIdAsync(userId);
        }
    }
}
