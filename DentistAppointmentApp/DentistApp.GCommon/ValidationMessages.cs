using System.Runtime.CompilerServices;

namespace DentistApp.GCommon
{
    public class ValidationMessages
    {
        
        public const string DuplicatedAppointmentTimeValidationMessage = "Duplicated Appointment Date/Time";
        public const string AppointmentCannotBeInThePastValidationMessage = "Appointment's Date and Time combination cannot be in the past";
        public const string AppointmentCannotBeCreatedWithoutDentistValidationMessage = "Error while creating Appointment. At least one dentist user should be configured";
        public const string AppointmentCannotBeFoundValidationMessage = "Appointment not found";

        public const string DuplicateManipulationNameValidationMessage = "Duplicated manipulation name";
        public const string ManipulationCannotBeFoundValidationMessage = "Manipulation not found";
        public const string ManipulationNotCorrectValidationMessage = "Manipulation Type is not correct";

        public const string ProcedureCannotBeInTheFutureValidationMessage = "Procedure's Date cannot be in the future";
        public const string ProcedureCreatorIsNotDentistValidationMessage = "Error while creating Procedure. The user is not a dentist";
        public const string ProcedureCreatorNotInDatabaseValidationMessage = "Error while creating Procedure. The user is not in the DataBase";
        public const string ProcedureCannotBeFoundValidationMessage = "Procedure not found";
        public const string ProcedureDentistNotInDatabaseValidationMessage = "Error while creating Procedure. The dentistId is not in the Databse";

    }
}
