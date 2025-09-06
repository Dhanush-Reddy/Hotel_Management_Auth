using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Bookings.Entities;

namespace Hotel.Application.Features.Bookings.Interfaces
{
    public interface IBookingService
    {
        Task<int> CreateAsync(int roomId, int guestId, DateTime startDate, DateTime endDate, decimal? nightlyRate, string? notes, int createdByUserId);
        Task<Booking?> GetAsync(int id);
        Task<IEnumerable<Booking>> ListAsync(int page = 1, int pageSize = 50, int? roomId = null, int? guestId = null, string? status = null, DateTime? from = null, DateTime? to = null);
        Task ConfirmAsync(int id);
        Task CheckInAsync(int id, DateTime todayUtc);
        Task CheckOutAsync(int id, DateTime todayUtc);
        Task CancelAsync(int id);
    }
}

