using System;

namespace Hotel.Domain.Features.Guests.Entities
{
    public class Guest
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? IdProof { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

