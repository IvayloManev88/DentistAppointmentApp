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
            if (procedureToCreate.ProcedureDate > dateTimeService.Today())
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
                .SoftDeleteProcedureAsync(procedureToDelete);
        }

        public async Task<ProcedurePaginationViewModel> GetAllProceduresViewModelsAsync(string userId, string? searchQuery=null, int page = 1)
        {
            int pageSize = ItemsPerPage;
            page = Math.Max(1, page);
            var (proceduresDto, totalCount) = await procedureRepository
                .GetPagedProceduresAsync(userId, searchQuery, page, pageSize);

            IEnumerable<ProcedureViewViewModel> procedures = proceduresDto
                .Select(p => new ProcedureViewViewModel
               {
                    ProcedureId = p.ProcedureId.ToString(),
                    PatientProcedureName = $"{p.PatientFirstName} {p.PatientLastName}",
                    DentistProcedureName = $"{p.DentistFirstName} {p.DentistLastName}",
                    ProcedureDate = p.ProcedureDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                    PatientProcedurePhoneNumber = p.PatientPhoneNumber,
                    ManipulationName = p.ManipulationName,
                    ProcedureDentistNote = p.DentistNote
                });

            ProcedurePaginationViewModel resultViewModel = new ProcedurePaginationViewModel
            {
                Procedures = procedures,
                SearchQuery = searchQuery,
                CurrentPage = page,
                ProceduresPerPage = pageSize,
                TotalItemsCount = totalCount
            };

            return resultViewModel;
        }

        public async Task<ProcedureCreateViewModel> GetCreateViewModelAsync()
        {
            ProcedureCreateViewModel createModel = new ProcedureCreateViewModel();
            createModel.ProcedureDate = dateTimeService.Today();
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
