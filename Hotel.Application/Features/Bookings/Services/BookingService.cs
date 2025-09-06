using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Application.Features.Bookings.Interfaces;
using Hotel.Application.Features.Rooms.Interfaces;

namespace Hotel.Application.Features.Bookings.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IRoomRepository _rooms;

        public BookingService(IBookingRepository repo, IRoomRepository rooms)
        {
            _repo = repo;
            _rooms = rooms;
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

        public async Task CheckInAsync(int id, DateTime todayUtc)
        {
            var b = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (!string.Equals(b.Status, "Confirmed", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Only Confirmed bookings can be checked in");
            var today = todayUtc.Date;
            if (!(b.StartDate <= today && today < b.EndDate)) throw new InvalidOperationException("Check-in not within stay window");
            await _repo.UpdateStatusAsync(id, "CheckedIn");
            await _rooms.SetStatusAsync(b.RoomId, "Occupied");
        }

        public async Task CheckOutAsync(int id, DateTime todayUtc)
        {
            var b = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (!string.Equals(b.Status, "CheckedIn", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Only CheckedIn bookings can be checked out");
            var today = todayUtc.Date;
            if (today < b.StartDate) throw new InvalidOperationException("Checkout cannot be before start");
            await _repo.UpdateStatusAsync(id, "CheckedOut");
            // naive: free room; smarter: check if another booking is active
            await _rooms.SetStatusAsync(b.RoomId, "Available");
        }

        public async Task CancelAsync(int id)
        {
            var b = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (string.Equals(b.Status, "CheckedOut", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(b.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot cancel this booking");
            await _repo.UpdateStatusAsync(id, "Cancelled");
        }
    }
}

