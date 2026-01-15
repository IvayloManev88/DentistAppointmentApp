namespace DentistApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using static DentistApp.Data.EntityConstants.EntityConstants.Manipulation;

    [Comment("Table defining the manipulation types")]
    public class ManipulationType
    {
        [Comment("Primary Key for the Manipulation type class")]
        [Key]
        [Required]
        public Guid ManipulationId { get; set; }

        [Comment("Name of the manipulation")]
        [StringLength(ManipulationNameMaxLength)]
        [Required]
        public string Name { get; set; } = null!;

        [StringLength(ExpectedPriceMaxLenght)]
        [Comment("Defined expected price range. The type string is defined because the expected values are 50-100 and should be informative for the customer/patient")]
        [Required]
        public string PriceRange { get; set; } = null!;

        [Comment("Collection of Reservations for each manipulation")]
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();

        [Comment("Collection of Procedures for each manipulation")]
        public virtual ICollection<Procedure> Procedures { get; set; } = new HashSet<Procedure>();

        [Comment("If the manipulation is deleted = false then it should not be selectable and visible")]
        public bool IsDeleted { get; set; } = false;

    }
}