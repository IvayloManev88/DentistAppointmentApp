namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Data.Models;
    using DentistApp.Services.Core.Contracts;
    using DentistApp.ViewModels;
    using DentistApp.ViewModels.ProcedureViewModels;

    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using static DentistApp.GCommon.GlobalCommon;
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

        public async Task CreateProcedureAsync(ProcedureCreateViewModel procedureToCreate, DateTime procedureDate)
        {
            bool isManipulationCorrect = await manipulationService.ValidateManipulationTypesAsync(procedureToCreate.ManipulationTypeId);
            if (!isManipulationCorrect)
            {
                throw new Exception("Manipulation Service is not correct");
            }
            if (procedureDate > DateTime.Today)
            {
                throw new Exception("Procedure's Date cannot be in the future");
            }
            string? dentistId = await patientService.GetDentistIdAsync();
            if (dentistId == null)
            {
                throw new Exception("Error while creating Procedure. At least one dentist user should be configured");
            }
            if (!await patientService.IsUserInDbByIdAsync(procedureToCreate.PatientId))
            {
                throw new Exception("Error while creating Procedure. The user is not in the DataBase");
            }

            Procedure currentProcedure = new Procedure
            {
                PatientId = procedureToCreate.PatientId.ToString(),
                DentistId = dentistId,
                Date = procedureDate,
                PatientPhoneNumber = procedureToCreate.PatientPhoneNumber,
                ManipulationTypeId = procedureToCreate.ManipulationTypeId,
                Note = procedureToCreate.DentistNote
            };

            await dbContext.Procedures.AddAsync(currentProcedure);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProcedureViewViewModel>> GetAllProceduresViewModelsAsync(string userId)
        {
            IEnumerable<ProcedureViewViewModel> procedures = await dbContext
               .Procedures
               .AsNoTracking()
               .Include(p => p.ManipulationType)
               .Include(p => p.Dentist)
               .Include(p => p.Patient)
               .Where(p => p.DentistId == userId ||
               p.PatientId == userId)
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

        public async Task<ProcedureCreateViewModel> GetCreateViewModelAsync()
        {
            IEnumerable<DropDown> manipulationTypes = await manipulationService.GetManipulationTypesAsync();
            ProcedureCreateViewModel createModel = new ProcedureCreateViewModel();
            createModel.ProcedureDate = DateTime.Today;
            createModel.ManipulationTypes =await manipulationService.GetManipulationTypesAsync();
            createModel.PatientsNames = await patientService.GetPatientsAsync();
            return createModel;
        }
    }
}
