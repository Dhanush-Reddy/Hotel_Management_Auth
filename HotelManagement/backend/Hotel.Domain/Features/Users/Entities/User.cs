using System;
using System.Text.Json.Serialization;

namespace Hotel.Domain.Features.Users.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        [JsonIgnore]
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Staff"; // Admin or Staff
        public bool ActiveFlag { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
