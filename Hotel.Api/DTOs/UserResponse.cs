using System;

namespace Hotel.Api.DTOs
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

