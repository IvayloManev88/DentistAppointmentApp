namespace DentistApp.ViewModels.FeedbackViewModels
{
    using System.ComponentModel.DataAnnotations;
    using static DentistApp.GCommon.EntityConstants.Feedback;
    public class FeedBackCreateViewModel
    {
        [StringLength(FeedbackTextMaxLength, MinimumLength =FeedbackTextMinLength,
            ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public string FeedbackText { get; set; } = null!;

        [Range(FeedbackMinRating, FeedbackMaxRating,
            ErrorMessage = "{0} must be between {1} and {2}.")]
        public int Rating { get; set; }
    }
}
