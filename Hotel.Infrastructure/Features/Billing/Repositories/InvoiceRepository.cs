using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Hotel.Application.Features.Billing.Interfaces;
using Hotel.Domain.Features.Billing.Entities;
using Hotel.Infrastructure.Common.Data;

namespace Hotel.Infrastructure.Features.Billing.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly SqlConnectionFactory _factory;
        public InvoiceRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Invoice invoice)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
INSERT INTO Invoices(BookingId, InvoiceNumber, Nights, NightlyRate, Subtotal, Status, CreatedAt, PaidAt)
VALUES(@BookingId, @InvoiceNumber, @Nights, @NightlyRate, @Subtotal, @Status, SYSUTCDATETIME(), NULL);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await conn.ExecuteScalarAsync<int>(sql, invoice);
        }

        public async Task UpdateInvoiceNumberAsync(int id, string number)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync("UPDATE Invoices SET InvoiceNumber=@number WHERE Id=@id;", new { id, number });
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Invoice>("SELECT * FROM Invoices WHERE Id=@Id;", new { Id = id });
        }

        public async Task<IEnumerable<Invoice>> ListAsync(int page = 1, int pageSize = 50, int? bookingId = null, string? status = null)
        {
            using var conn = _factory.CreateConnection();
            var offset = (page - 1) * pageSize;
            var sql = @"
SELECT * FROM Invoices
WHERE (@bookingId IS NULL OR BookingId = @bookingId)
  AND (@status IS NULL OR Status = @status)
ORDER BY Id DESC
OFFSET @offset ROWS FETCH NEXT @ps ROWS ONLY;";
            return await conn.QueryAsync<Invoice>(sql, new { bookingId, status, offset, ps = pageSize });
        }

        public async Task MarkPaidAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(@"UPDATE Invoices SET Status='Paid', PaidAt=SYSUTCDATETIME() WHERE Id=@id;", new { id });
        }

        public async Task<bool> ExistsForBookingAsync(int bookingId)
        {
            using var conn = _factory.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Invoices WHERE BookingId=@bookingId;", new { bookingId });
            return count > 0;
        }
    }
}

