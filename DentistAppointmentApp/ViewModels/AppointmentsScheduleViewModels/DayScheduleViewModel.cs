using System.Collections.Generic;

namespace DentistApp.ViewModels.AppointmentsScheduleViewModels
{
    public class DayScheduleViewModel
    {
        public DateTime Date { get; set; }

        public List<AppointmentScheduleItemViewModel> Appointments { get; set; } = new List<AppointmentScheduleItemViewModel> ();

        public List<TimeSlotViewModel> FreeSlots { get; set; } = new List<TimeSlotViewModel> ();
    }
}
