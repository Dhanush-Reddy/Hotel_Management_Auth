using System;

namespace Hotel.Api.Features.Guests.DTOs
{
    public class GuestResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? IdProof { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

