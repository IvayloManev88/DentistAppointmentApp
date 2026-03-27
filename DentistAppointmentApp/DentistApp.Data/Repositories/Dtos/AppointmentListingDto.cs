namespace DentistApp.Data.Repositories.Dtos
{
    public class AppointmentListingDto
    {
        public Guid AppointmentId { get; set; }

        public string PatientFirstName { get; set; } = null!;

        public string PatientLastName { get; set; } = null!;

        public string DentistFirstName { get; set; } = null!;

        public string DentistLastName { get; set; } = null!;

        public DateTime AppointmentDate { get; set; }

        public string PatientPhoneNumber { get; set; } = null!;

        public string ManipulationName { get; set; } = null!;

        public string? AppointmentNote { get; set; } = null!;

        public string PatientId { get; set; } = null!;
    }
}
