namespace DentistApp.ViewModels.AppointmentViewModels
{
    public class AppointmentViewAppointmentViewModel
    {
        public string AppointmentId { get; set; } = null!;
        public string PatientAppointmentName { get; set; } = null!;
        public string DentistAppointmentName { get; set; } = null!;
        public string AppointmentDate { get; set; } = null!;
        public string PatientAppointmentPhoneNumber { get; set; } = null!;
        public string ManipulationName { get; set; } = null!;
        public string? AppointmentNote { get; set; } 
        public string AppointmentUserCreated { get; set; } = null!;
    }
}
