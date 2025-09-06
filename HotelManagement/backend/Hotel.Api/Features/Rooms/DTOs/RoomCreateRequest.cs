namespace Hotel.Api.Features.Rooms.DTOs
{
    public class RoomCreateRequest
    {
        public string RoomNumber { get; set; } = "";
        public int Capacity { get; set; } = 2;
    }
}

