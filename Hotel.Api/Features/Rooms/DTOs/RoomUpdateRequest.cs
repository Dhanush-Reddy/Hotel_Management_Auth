namespace Hotel.Api.Features.Rooms.DTOs
{
    public class RoomUpdateRequest
    {
        public string RoomNumber { get; set; } = "";
        public int Capacity { get; set; } = 2;
    }
}

