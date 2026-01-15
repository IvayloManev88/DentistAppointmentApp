namespace DentistApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Comment("Table defining the Reservations made")]
    //Side note - was considering making abstract class for matching properties between Reservation and Procedure entities but it is better to have them separate as "Note" for example does something different
    public class Reservation
    {
        [Comment("Primary Key for the Reservations type class")]
        [Key]
        [Required]
        public Guid ReservationId { get; set; }

        [Comment("Id of the Patient to which the procedure was done on")]
        [Required]
        public string PatientId { get; set; } = null!;

        [Comment("Date and time of the procedure")]
        [Required]
        public DateTime Date { get; set; }

        [Comment("Id of the Manipulation performed")]
        [Required]
        public Guid ManipulationTypeId { get; set; }
             
        [Comment("Note left by the patient for the dentist while making reservation")]
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

    }

}
