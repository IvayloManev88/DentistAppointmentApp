namespace DentistApp.ViewModels.ProcedureViewModels
{
    using System.ComponentModel.DataAnnotations;

    using static DentistApp.GCommon.ProcedureConstants;

    public class ProcedureCreateViewModel
    {
        public Guid? ProcedureId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ProcedureDate { get; set; }
        public IEnumerable<DropDown> PatientsNames { get; set; } = Enumerable.Empty<DropDown>();
        [Required]
        public string PatientId { get; set; } = null!;

        [Required]
        public Guid ManipulationTypeId { get; set; }

        public IEnumerable<DropDown> ManipulationTypes { get; set; } = Enumerable.Empty<DropDown>();

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
