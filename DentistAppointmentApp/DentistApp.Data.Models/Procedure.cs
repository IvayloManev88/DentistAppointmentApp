
namespace DentistApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static DentistApp.GCommon.EntityConstants.Appointment;

    [Comment("Table defining the Procedures done.")]
    //Side note - was considering making abstract class for matching properties between Appointment and Procedure entities but it is better to have them separate as "Note" for example does something different
    public class Procedure
    {
        [Comment("Primary Key for the Procedure type class")]
        [Key]
        [Required]
        public Guid ProcedureId { get; set; }

        [Comment("Id of the Patient to which the procedure was done on")]
        [Required]
        public string PatientId { get; set; } = null!;

        [Comment("Patient Phone number")]
        [MaxLength(PhoneNumberMaxLenght)]
        [Required]
        public string PatientPhoneNumber { get; set; } = null!;

        [Comment("Id of the Dentist that will perform the  procedure")]
        [Required]
        public string DentistId { get; set; } = null!;


        [Comment("Date and time of the procedure")]
        [Required]
        public DateTime Date { get; set; }

        [Comment("Id of the Manipulation performed")]
        [Required]
        public Guid ManipulationTypeId { get; set; }

        [Comment("Note left by the dentist after the procedure")]
        [MaxLength(PhoneNumberMaxLenght)]
        public string? Note { get; set; }

        [Comment("If the procedure's IsDeleted is set to true it will not be displayed in the system")]
        [Required]
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
