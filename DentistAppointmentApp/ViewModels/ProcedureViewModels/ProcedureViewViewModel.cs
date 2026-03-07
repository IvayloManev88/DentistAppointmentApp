namespace DentistApp.ViewModels.ProcedureViewModels
{
    public class ProcedureViewViewModel
    {
        public string ProcedureId { get; set; } = null!;
        public string PatientProcedureName { get; set; } = null!;
        public string DentistProcedureName { get; set; } = null!;
        public string ProcedureDate { get; set; } = null!;
        public string PatientProcedurePhoneNumber { get; set; } = null!;
        public string ManipulationName { get; set; } = null!;
        public string? ProcedureDentistNote { get; set; }
    }
}
