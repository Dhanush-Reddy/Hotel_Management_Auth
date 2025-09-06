using System;

namespace Hotel.Domain.Features.Billing.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string InvoiceNumber { get; set; } = ""; // e.g., INV-2025-00001
        public int Nights { get; set; }
        public decimal? NightlyRate { get; set; }
        public decimal Subtotal { get; set; }          // Nights * NightlyRate (null => 0)
        public string Status { get; set; } = "Unpaid"; // Unpaid | Paid
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}

