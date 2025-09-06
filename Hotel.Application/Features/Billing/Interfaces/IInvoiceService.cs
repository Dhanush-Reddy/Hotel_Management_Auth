using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Billing.Entities;

namespace Hotel.Application.Features.Billing.Interfaces
{
    public interface IInvoiceService
    {
        Task<int> CreateForBookingAsync(int bookingId);
        Task<Invoice?> GetAsync(int id);
        Task<IEnumerable<Invoice>> ListAsync(int page = 1, int pageSize = 50, int? bookingId = null, string? status = null);
        Task MarkPaidAsync(int id);
    }
}

