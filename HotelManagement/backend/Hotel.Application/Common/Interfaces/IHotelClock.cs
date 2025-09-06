namespace Hotel.Application.Common.Interfaces
{
    // Provides the hotel's current local date based on configured timezone
    public interface IHotelClock
    {
        System.DateTime Today(); // date-only (time set to 00:00) in hotel's local time
    }
}

