using System;

namespace Hotel.Api.Features.Users.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
        public bool ActiveFlag { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
