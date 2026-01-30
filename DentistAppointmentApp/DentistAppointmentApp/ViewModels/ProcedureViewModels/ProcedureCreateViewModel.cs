using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static DentistApp.GCommon.ProcedureConstants;

namespace DentistApp.Web.ViewModels.ProcedureViewModels
{
    public class ProcedureCreateViewModel
    {
        public string? ProcedureId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ProcedureDate { get; set; }
        public IEnumerable<SelectListItem> PatientsNames { get; set; } = Enumerable.Empty<SelectListItem>();
        [Required]
        public string PatientId { get; set; } = null!;

        [Required]
        public Guid ManipulationTypeId { get; set; }

        public IEnumerable<SelectListItem> ManipulationTypes { get; set; } = Enumerable.Empty<SelectListItem>();

        [Required]
        [RegularExpression(PhoneRegexValidation,
                ErrorMessage = "Please enter a value that starts with +359 or 0 and is followed by 9 digits.")]
        [StringLength(PhoneNumberMaxLength,
            ErrorMessage = "Please enter a valid phone number")]
        public string PatientPhoneNumber { get; set; } = null!;

        [StringLength(NoteMaxLength,
          ErrorMessage = "Note length cannot exceed 450 characters")]
        public string? DentistNote { get; set; }
    }
}
