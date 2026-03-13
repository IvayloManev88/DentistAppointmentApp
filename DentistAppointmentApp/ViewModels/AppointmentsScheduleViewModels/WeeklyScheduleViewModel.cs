namespace DentistApp.ViewModels.AppointmentsScheduleViewModels
{
    public class WeeklyScheduleViewModel
    {
            public DateTime WeekStartDate { get; set; }

            public List<DayScheduleViewModel> Days { get; set; } = new List<DayScheduleViewModel>();
        
    }
}
