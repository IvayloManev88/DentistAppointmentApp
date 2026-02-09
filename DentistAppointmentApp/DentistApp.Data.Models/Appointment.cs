namespace DentistApp.Data.Models
{
    using static DentistApp.GCommon.EntityConstants.Appointment;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.ComponentModel.DataAnnotations;

    [Comment("Table defining the Appointments made")]
    //Side note - was considering making abstract class for matching properties between Appointment and Procedure entities but it is better to have them separate as "Note" for example does something different
    public class Appointment
    {
        [Comment("Primary Key for the Appointments type class")]
        [Key]
        [Required]
        public Guid AppointmentId { get; set; }

        [Comment("Id of the Patient to which the procedure was done on")]
        [Required]
        public string PatientId { get; set; } = null!;

        [Comment("Id of the Dentist that will perform the  procedure")]
        [Required]
        public string DentistId { get; set; } = null!;

        [Comment("Date and time of the procedure")]
        [Required]
        public DateTime Date { get; set; }
        //I do not take the phone number from User entity because the user could be making Appointment for a third party.
        [Comment("Patient Phone number")]
        [MaxLength(PhoneNumberMaxLenght)]
        [Required]
        public string PatientPhoneNumber { get; set; } = null!;

        [Comment("Id of the Manipulation performed")]
        [Required]
        public Guid ManipulationTypeId { get; set; }
             
        [Comment("Note left by the patient for the dentist while making Appointment")]
        [MaxLength(NoteMaxLength)]
        public string? Note { get; set; }

        public bool IsConfirmed { get; set; } = true;

        [Comment("If the procedure's IsDeleted is set to true it will not be displayed in the system")]
        public bool IsDeleted { get; set; } = false;

        [Comment("Navigation property to the Patient")]
        [Required]
        public virtual ApplicationUser Patient { get; set; } = null!;

        [Comment("Navigation property to the Manipulation")]
        [Required]
        public virtual ManipulationType ManipulationType { get; set; } = null!;

        [Comment("Navigation property to the Dentist")]
        [Required]
        public virtual ApplicationUser Dentist { get; set; } = null!;

    }

}
