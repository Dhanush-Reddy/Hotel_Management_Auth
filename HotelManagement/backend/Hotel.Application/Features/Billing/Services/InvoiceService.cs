using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Application.Features.Billing.Interfaces;
using Hotel.Application.Features.Bookings.Interfaces;
using Hotel.Domain.Features.Billing.Entities;

namespace Hotel.Application.Features.Billing.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repo;
        private readonly IBookingRepository _bookings;

        public InvoiceService(IInvoiceRepository repo, IBookingRepository bookings)
        {
            _repo = repo;
            _bookings = bookings;
        }

        public async Task<int> CreateForBookingAsync(int bookingId)
        {
            var b = await _bookings.GetByIdAsync(bookingId) ?? throw new KeyNotFoundException("Booking not found");
            if (!string.Equals(b.Status, "CheckedOut", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invoice can only be created for CheckedOut bookings");
            if (await _repo.ExistsForBookingAsync(bookingId))
                throw new InvalidOperationException("Invoice already exists for this booking");

            var nights = b.Nights; // computed by DB in repo GetById
            var rate = b.NightlyRate ?? 0m;
            var subtotal = nights * rate;

            var invoice = new Invoice
            {
                BookingId = bookingId,
                InvoiceNumber = "TEMP", // will be replaced after insert
                Nights = nights,
                NightlyRate = b.NightlyRate,
                Subtotal = subtotal,
                Status = "Unpaid",
                CreatedAt = DateTime.UtcNow,
                PaidAt = null
            };

            var id = await _repo.CreateAsync(invoice);

            // Generate a friendly number after we know Id
            var number = $"INV-{DateTime.UtcNow:yyyy}-{id:D5}";
            await _repo.UpdateInvoiceNumberAsync(id, number);

            return id;
        }

        public Task<Invoice?> GetAsync(int id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Invoice>> ListAsync(int page = 1, int pageSize = 50, int? bookingId = null, string? status = null)
            => _repo.ListAsync(page, pageSize, bookingId, status);

        public Task MarkPaidAsync(int id) => _repo.MarkPaidAsync(id);
    }
}

