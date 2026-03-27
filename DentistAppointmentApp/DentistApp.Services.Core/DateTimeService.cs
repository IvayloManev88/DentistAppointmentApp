using DentistApp.Services.Core.Contracts;

namespace DentistApp.Services.Core
{
    public class DateTimeService : IDateTimeService
    {
        public TimeSpan GetTime()
        {
            return new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
        }

        public DateTime Today()
        {
            return DateTime.Today;
        }
    }
}
