namespace DentistApp.Data.Repositories.Dtos.ProcedureDtos
{
    public class ProcedureListingDto
    {
        public Guid ProcedureId { get; set; }

        public string PatientFirstName { get; set; } = null!;
        public string PatientLastName { get; set; } = null!;
        public string DentistFirstName { get; set; } = null!;
        public string DentistLastName { get; set; } = null!;
        public DateTime ProcedureDate { get; set; }
        public string PatientPhoneNumber { get; set; } = null!;
        public string ManipulationName { get; set; } = null!;
        public string? DentistNote { get; set; }
    }
}
