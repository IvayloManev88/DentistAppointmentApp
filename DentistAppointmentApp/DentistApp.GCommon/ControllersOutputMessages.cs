namespace DentistApp.GCommon
{
    public class ControllersOutputMessages
    {
        public const string ManipulationIsIncorrect = "The selected manipulation is incorrect";
        public const string ManipulationNameDuplication = "Duplicate manipulation name";
        public const string ManipulationCreationError = "An error occurred while creating Manipulation.Please try again!";

        public const string DentistUserNotConfigured = "Dentist user is not configured. Please contact the clinic";

        public const string AppointmentDateTimeTaken = "The selected combination Date/Time is already taken. Please try different Date/Time";
        public const string AppointmentSetInThePast = "You should not set an appintment in the past";
        public const string AppointmentCreationError = "An error occurred while creating Appointment. Please try again!";

        public const string ProcedurePatientIsIncorrect = "The selected patient is incorrect";
        public const string ProcedureSetInTheFuture = "You should not set procedure that is done in the future";
        public const string ProcedureUserIsNotInDatabase = "Current User is not in the Database";
        public const string ProcedureCreationError = "An error occurred while creating Procedure.Please try again!";




    }
}
