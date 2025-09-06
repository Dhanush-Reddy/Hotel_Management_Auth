using System;

namespace Hotel.Api.Features.Bookings.DTOs
{
    public class InlineGuestDto
    {
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? IdProof { get; set; }
    }

    public class BookingCreateRequest
    {
        public int RoomId { get; set; }
        public int? GuestId { get; set; }      // optional if Guest is provided
        public InlineGuestDto? Guest { get; set; }
        public DateTime StartDate { get; set; } // supply YYYY-MM-DD
        public DateTime EndDate { get; set; }   // exclusive
        public decimal? NightlyRate { get; set; }
        public string? Notes { get; set; }
    }
}

