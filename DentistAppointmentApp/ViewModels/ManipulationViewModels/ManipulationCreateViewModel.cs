namespace DentistApp.ViewModels.ManipulationViewModels
{
    using System.ComponentModel.DataAnnotations;

    using static DentistApp.GCommon.EntityConstants.Manipulation;
    public class ManipulationCreateViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(ManipulationNameMaxLength,
            MinimumLength =ManipulationNameMinLength, 
            ErrorMessage = "Name of the manipulation should be between {2} and {1} characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price range is required.")]
        [StringLength(ExpectedPriceMaxLenght,
            ErrorMessage = "Price range cannot be longer than {1} characters.")]
        [RegularExpression(@"^\d{1,4}(-\d{1,6})?$", ErrorMessage = "Price range in a valid format: Example \"50-100\" (no spaces).")]
        public string PriceRange { get; set; } = null!;
    }
}
