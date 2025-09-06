namespace Hotel.Api.Features.Rooms.DTOs
{
    public class RoomStatusRequest
    {
        public string Status { get; set; } = "Available"; // Available | Occupied | OutOfService
    }
}

