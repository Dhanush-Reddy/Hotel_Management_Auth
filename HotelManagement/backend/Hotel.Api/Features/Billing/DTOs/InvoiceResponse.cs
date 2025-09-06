using System;

namespace Hotel.Api.Features.Billing.DTOs
{
    public class InvoiceResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public int BookingId { get; set; }
        public int Nights { get; set; }
        public decimal? NightlyRate { get; set; }
        public decimal Subtotal { get; set; }
        public string Status { get; set; } = "Unpaid";
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}

