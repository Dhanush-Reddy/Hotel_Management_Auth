using System;

namespace Hotel.Domain.Features.Bookings.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public DateTime StartDate { get; set; }   // use date part only
        public DateTime EndDate { get; set; }     // exclusive
        public string Status { get; set; } = "Pending"; // Pending|Confirmed|CheckedIn|CheckedOut|Cancelled
        public decimal? NightlyRate { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Nights { get; set; } // computed by DB but can be hydrated
    }
}

