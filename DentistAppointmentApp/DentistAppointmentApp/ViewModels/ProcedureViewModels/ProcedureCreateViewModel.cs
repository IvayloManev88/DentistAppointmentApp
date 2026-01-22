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
        [RegularExpression(PhoneRegexValidation)]
        [StringLength(PhoneNumberMaxLength)]
        public string PatientPhoneNumber { get; set; } = null!;

        [StringLength(NoteMaxLength)]
        public string? DentistNote { get; set; }
    }
}
