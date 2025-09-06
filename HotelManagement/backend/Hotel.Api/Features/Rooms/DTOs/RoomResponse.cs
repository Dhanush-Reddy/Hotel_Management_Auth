using System;

namespace Hotel.Api.Features.Rooms.DTOs
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = "";
        public int Capacity { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; }
    }
}

