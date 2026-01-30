namespace DentistApp.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using static DentistApp.Data.EntityConstants.EntityConstants.ApplicationUser;

    [Comment("Dentist User Entity in the system")]
    public class ApplicationUser : IdentityUser
    {

        [Comment("User's first name")]
        [Required]
        [MaxLength(ApplicationUserFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Comment("User's last name")]
        [Required]
        [MaxLength(ApplicationUserLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        [Comment("User's Appointments collection")]
        public virtual ICollection<Appointment> PatientAppointments { get; set; } = new HashSet<Appointment>();

        [Comment("User's Appointments collection")]
        public virtual ICollection<Appointment> DentistAppointments { get; set; } = new HashSet<Appointment>();



        [Comment("User's Procedures done collection")]
        public virtual ICollection<Procedure> PatientProcedures { get; set; } = new HashSet<Procedure>();

        [Comment("User's Procedures done collection")]
        public virtual ICollection<Procedure> DentistProcedures { get; set; } = new HashSet<Procedure>();

        [Comment("If the user is deleted = false then he/she is an active user")]
        public bool IsDeleted { get; set; } = false;

    }
}

