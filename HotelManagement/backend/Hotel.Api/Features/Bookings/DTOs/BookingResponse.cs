using System;

namespace Hotel.Api.Features.Bookings.DTOs
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int GuestId { get; set; }
        public string? GuestName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }   // exclusive
        public string Status { get; set; } = "";
        public decimal? NightlyRate { get; set; }
        public int Nights { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

