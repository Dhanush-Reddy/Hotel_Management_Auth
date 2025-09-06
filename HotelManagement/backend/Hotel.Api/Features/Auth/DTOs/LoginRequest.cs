namespace Hotel.Api.Features.Auth.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
