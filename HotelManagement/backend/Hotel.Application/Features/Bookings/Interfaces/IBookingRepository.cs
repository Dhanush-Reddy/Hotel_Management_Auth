using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Bookings.Entities;

namespace Hotel.Application.Features.Bookings.Interfaces
{
    public interface IBookingRepository
    {
        Task<int> CreateAsync(Booking b); // must enforce overlap (transaction)
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> ListAsync(int page = 1, int pageSize = 50, int? roomId = null, int? guestId = null, string? status = null, DateTime? from = null, DateTime? to = null);
        Task<bool> ExistsOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeBookingId = null);
        Task UpdateStatusAsync(int id, string status);
    }
}

