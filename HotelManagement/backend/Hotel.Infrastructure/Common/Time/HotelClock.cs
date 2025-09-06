using System;
using Hotel.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Hotel.Infrastructure.Common.Time
{
    public class HotelClock : IHotelClock
    {
        private readonly TimeZoneInfo _tz;

        public HotelClock(IConfiguration config)
        {
            var tzId = config["Hotel:TimeZoneId"] ?? "UTC";
            try { _tz = TimeZoneInfo.FindSystemTimeZoneById(tzId); }
            catch { _tz = TimeZoneInfo.Utc; }
        }

        public DateTime Today()
        {
            var nowUtc = DateTime.UtcNow;
            var local = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, _tz);
            return new DateTime(local.Year, local.Month, local.Day, 0, 0, 0, DateTimeKind.Unspecified);
        }
    }
}

