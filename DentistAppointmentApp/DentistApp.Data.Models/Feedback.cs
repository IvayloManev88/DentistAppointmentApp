namespace DentistApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    using System.ComponentModel.DataAnnotations;

    using static DentistApp.GCommon.EntityConstants.Feedback;
    public class Feedback
    {
        [Comment("Primary Key for the Patient Feedback type class")]
        [Key]
        public Guid FeedbackId { get; set; }

        [Comment("Date and Time when the feedback was left on the site")]
        public DateTime CreatedOn { get; set; }

        [Comment("Actual feedback message")]
        [Required]
        [StringLength(FeedbackTextMaxLength)]
        public string FeedbackText { get; set; } = null!;

        [Comment("Rating will be used for getting the overall rating score")]
        [Range(FeedbackMinRating, FeedbackMaxRating)]
        public int Rating { get; set; }

        [Comment("Id of the Procedure for which we are leaving feedback")]
        [Required]
        public Guid ProcedureId { get; set; }

        [Comment("If the feedback's IsDeleted is set to true it will not be displayed in the system")]
        public bool IsDeleted { get; set; } = false;

        [Comment("Navigation property to the Procedure")]
        [Required]
        public virtual Procedure Procedure { get; set; } = null!;
        
    }

}
