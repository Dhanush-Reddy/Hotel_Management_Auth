using System;

namespace Hotel.Domain.Features.Rooms.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = "";
        public int Capacity { get; set; } = 2;
        public string Status { get; set; } = "Available"; // Available | Occupied | OutOfService
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

