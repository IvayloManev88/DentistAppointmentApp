namespace DentistApp.Services.Core
{
    using DentistApp.Data;
    using DentistApp.Services.Core.Contracts;
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
    }
}
