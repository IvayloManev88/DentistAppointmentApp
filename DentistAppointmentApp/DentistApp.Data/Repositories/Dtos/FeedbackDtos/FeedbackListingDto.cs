using System;
using System.Collections.Generic;
using System.Text;

namespace DentistApp.Data.Repositories.Dtos.FeedbackDtos
{
    public class FeedbackListingDto
    {
        public string CreatedOn { get; set; } = null!;
        public string FeedbackText { get; set; } = null!;
        public int Rating { get; set; }
        public string ProcedureManipulationType { get; set; } = null!;
        public string ProcedurePatientName { get; set; } = null!;
    }
}
