using System;

namespace Hotel.Api.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }
    }
}

