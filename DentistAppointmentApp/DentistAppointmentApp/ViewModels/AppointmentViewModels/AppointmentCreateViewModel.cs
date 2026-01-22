namespace DentistApp.Web.ViewModels.AppointmentViewModels
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;
    using static DentistApp.GCommon.AppointmentConstants;
    public class AppointmentCreateViewModel
    {
        public string? AppointmentId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        public Guid ManipulationTypeId { get; set; }

        public IEnumerable<SelectListItem> ManipulationTypes { get; set; } = Enumerable.Empty<SelectListItem>();   

        [Required]
        [RegularExpression(PhoneRegexValidation)]
        [StringLength(PhoneNumberMaxLength)]
        public string PatientPhoneNumber { get; set; } = null!;

        [StringLength(NoteMaxLength)]
        public string? Note { get; set; }

    }
}
