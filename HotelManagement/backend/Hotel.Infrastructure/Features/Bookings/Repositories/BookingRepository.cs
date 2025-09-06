using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Hotel.Application.Features.Bookings.Interfaces;
using Hotel.Domain.Features.Bookings.Entities;
using Hotel.Infrastructure.Common.Data;

namespace Hotel.Infrastructure.Features.Bookings.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly SqlConnectionFactory _factory;
        public BookingRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Booking b)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction(IsolationLevel.Serializable);

            // overlap check (exclusive EndDate)
            var conflictSql = @"
SELECT COUNT(1)
FROM Bookings
WHERE RoomId = @RoomId
  AND Status IN ('Pending','Confirmed','CheckedIn')
  AND (@StartDate < EndDate AND StartDate < @EndDate);";

            var conflicts = await conn.ExecuteScalarAsync<int>(conflictSql, new { b.RoomId, b.StartDate, b.EndDate }, tx);
            if (conflicts > 0)
            {
                tx.Rollback();
                throw new InvalidOperationException("Overlap detected");
            }

            var insertSql = @"
INSERT INTO Bookings(RoomId, GuestId, StartDate, EndDate, Status, NightlyRate, Notes, CreatedByUserId, CreatedAt)
VALUES(@RoomId, @GuestId, @StartDate, @EndDate, @Status, @NightlyRate, @Notes, @CreatedByUserId, SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await conn.ExecuteScalarAsync<int>(insertSql, b, tx);
            tx.Commit();
            return id;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            // return core fields; joins can be done in controller/service if needed separately
            var sql = @"SELECT *, DATEDIFF(DAY, StartDate, EndDate) AS Nights FROM Bookings WHERE Id=@Id;";
            return await conn.QueryFirstOrDefaultAsync<Booking>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Booking>> ListAsync(int page = 1, int pageSize = 50, int? roomId = null, int? guestId = null, string? status = null, DateTime? from = null, DateTime? to = null)
        {
            using var conn = _factory.CreateConnection();
            var offset = (page - 1) * pageSize;

            var sql = @"
SELECT *, DATEDIFF(DAY, StartDate, EndDate) AS Nights
FROM Bookings
WHERE (@roomId IS NULL OR RoomId = @roomId)
  AND (@guestId IS NULL OR GuestId = @guestId)
  AND (@status IS NULL OR Status = @status)
  AND (
        (@from IS NULL AND @to IS NULL)
        OR (@from IS NOT NULL AND @to IS NULL AND EndDate > @from)
        OR (@from IS NULL AND @to IS NOT NULL AND StartDate < @to)
        OR (@from IS NOT NULL AND @to IS NOT NULL AND @from < EndDate AND StartDate < @to)
      )
ORDER BY Id DESC
OFFSET @offset ROWS FETCH NEXT @ps ROWS ONLY;";

            return await conn.QueryAsync<Booking>(sql, new { roomId, guestId, status, from, to, offset, ps = pageSize });
        }

        public async Task<bool> ExistsOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeBookingId = null)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
SELECT COUNT(1)
FROM Bookings
WHERE RoomId = @roomId
  AND Status IN ('Pending','Confirmed','CheckedIn')
  AND (@start < EndDate AND StartDate < @end)
  AND (@excludeId IS NULL OR Id <> @excludeId);";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { roomId, start, end, excludeId = excludeBookingId });
            return count > 0;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync("UPDATE Bookings SET Status=@status WHERE Id=@id;", new { id, status });
        }
    }
}
