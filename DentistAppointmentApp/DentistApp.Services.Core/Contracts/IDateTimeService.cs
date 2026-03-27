namespace DentistApp.Services.Core.Contracts
{
    public interface IDateTimeService
    {
        DateTime Today();

        TimeSpan GetTime();
    }
}
