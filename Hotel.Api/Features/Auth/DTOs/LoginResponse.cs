using System;

namespace Hotel.Api.Features.Auth.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }
    }
}
