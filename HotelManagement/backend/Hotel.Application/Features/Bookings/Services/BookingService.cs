using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Application.Features.Bookings.Interfaces;
using Hotel.Application.Features.Rooms.Interfaces;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Features.Billing.Interfaces;

namespace Hotel.Application.Features.Bookings.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IRoomRepository _rooms;
        private readonly IHotelClock _clock;
        private readonly IInvoiceService _invoices;

        public BookingService(IBookingRepository repo, IRoomRepository rooms, IHotelClock clock, IInvoiceService invoices)
        {
            _repo = repo;
            _rooms = rooms;
            _clock = clock;
            _invoices = invoices;
        }

        public async Task<int> CreateAsync(int roomId, int guestId, DateTime startDate, DateTime endDate, decimal? nightlyRate, string? notes, int createdByUserId)
        {
            if (startDate.Date >= endDate.Date) throw new ArgumentException("StartDate must be before EndDate");
            // Ensure no overlap
            var hasOverlap = await _repo.ExistsOverlapAsync(roomId, startDate.Date, endDate.Date, null);
            if (hasOverlap) throw new InvalidOperationException("Room is already booked for the selected dates");

            var b = new Domain.Features.Bookings.Entities.Booking {
                RoomId = roomId, GuestId = guestId,
                StartDate = startDate.Date, EndDate = endDate.Date,
                Status = "Pending", NightlyRate = nightlyRate, Notes = notes,
                CreatedByUserId = createdByUserId, CreatedAt = DateTime.UtcNow
            };
            return await _repo.CreateAsync(b);
        }

        public Task<Domain.Features.Bookings.Entities.Booking?> GetAsync(int id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Domain.Features.Bookings.Entities.Booking>> ListAsync(int page = 1, int pageSize = 50, int? roomId = null, int? guestId = null, string? status = null, DateTime? from = null, DateTime? to = null)
            => _repo.ListAsync(page, pageSize, roomId, guestId, status, from?.Date, to?.Date);

        public async Task ConfirmAsync(int id)
        {
            var b = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (!string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Only Pending bookings can be confirmed");
            var overlap = await _repo.ExistsOverlapAsync(b.RoomId, b.StartDate, b.EndDate, id);
            if (overlap) throw new InvalidOperationException("Room is already booked for the selected dates");
            await _repo.UpdateStatusAsync(id, "Confirmed");
        }

        public async Task CheckInAsync(int id)
        {
            var b = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (!string.Equals(b.Status, "Confirmed", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Only Confirmed bookings can be checked in");
            await _repo.UpdateStatusAsync(id, "CheckedIn");
        }

        public async Task CheckOutAsync(int id)
        {
            var b = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (!string.Equals(b.Status, "CheckedIn", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Only CheckedIn bookings can be checked out");
            await _repo.UpdateStatusAsync(id, "CheckedOut");
            try { await _invoices.CreateForBookingAsync(id); } catch { }
        }

        public async Task CancelAsync(int id)
        {
            var b = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (string.Equals(b.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Booking already cancelled");
            await _repo.UpdateStatusAsync(id, "Cancelled");
        }

        public async Task UpdateAsync(int id, int roomId, int guestId, DateTime startDate, DateTime endDate, decimal? nightlyRate, string? notes)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (startDate.Date >= endDate.Date) throw new ArgumentException("StartDate must be before EndDate");
            // Allow edits only when not Completed/Cancelled
            if (string.Equals(existing.Status, "CheckedOut", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(existing.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot edit this booking");

            var updated = new Domain.Features.Bookings.Entities.Booking
            {
                Id = id,
                RoomId = roomId,
                GuestId = guestId,
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                Status = existing.Status,
                NightlyRate = nightlyRate,
                Notes = notes,
                CreatedByUserId = existing.CreatedByUserId,
                CreatedAt = existing.CreatedAt
            };
            await _repo.UpdateAsync(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            var isCancelled = string.Equals(existing.Status, "Cancelled", StringComparison.OrdinalIgnoreCase);
            var isCheckedOut = string.Equals(existing.Status, "CheckedOut", StringComparison.OrdinalIgnoreCase);
            if (!isCancelled && !isCheckedOut)
                throw new InvalidOperationException("Only cancelled or checked-out bookings can be deleted");
            await _repo.DeleteAsync(id);
        }
    }
}
