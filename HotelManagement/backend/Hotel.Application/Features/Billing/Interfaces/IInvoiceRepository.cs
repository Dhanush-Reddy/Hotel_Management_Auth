using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Billing.Entities;

namespace Hotel.Application.Features.Billing.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<int> CreateAsync(Invoice invoice);              // inserts with placeholder number, returns Id
        Task UpdateInvoiceNumberAsync(int id, string number);
        Task<Invoice?> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> ListAsync(int page = 1, int pageSize = 50, int? bookingId = null, string? status = null);
        Task MarkPaidAsync(int id);
        Task<bool> ExistsForBookingAsync(int bookingId);
    }
}

