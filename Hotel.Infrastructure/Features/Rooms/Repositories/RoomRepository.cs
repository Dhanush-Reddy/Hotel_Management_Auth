using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Hotel.Application.Features.Rooms.Interfaces;
using Hotel.Domain.Features.Rooms.Entities;
using Hotel.Infrastructure.Common.Data;

namespace Hotel.Infrastructure.Features.Rooms.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly SqlConnectionFactory _factory;
        public RoomRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<Room>> ListAsync(int page = 1, int pageSize = 50, string? q = null, string? status = null)
        {
            using var conn = _factory.CreateConnection();
            var offset = (page - 1) * pageSize;
            var like = string.IsNullOrWhiteSpace(q) ? null : $"%{q}%";

            var sql = @"
SELECT * FROM Rooms
WHERE (@q IS NULL OR RoomNumber LIKE @like)
  AND (@status IS NULL OR Status = @status)
ORDER BY Id DESC
OFFSET @offset ROWS FETCH NEXT @ps ROWS ONLY;";

            return await conn.QueryAsync<Room>(sql, new { q, like, status, offset, ps = pageSize });
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Room>(
                "SELECT TOP 1 * FROM Rooms WHERE Id=@Id", new { Id = id });
        }

        public async Task<int> CreateAsync(Room room)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
INSERT INTO Rooms(RoomNumber, Capacity, Status)
VALUES(@RoomNumber, @Capacity, @Status);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await conn.ExecuteScalarAsync<int>(sql, room);
        }

        public async Task UpdateAsync(Room room)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"UPDATE Rooms SET RoomNumber=@RoomNumber, Capacity=@Capacity WHERE Id=@Id;";
            await conn.ExecuteAsync(sql, room);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync("DELETE FROM Rooms WHERE Id=@Id;", new { Id = id });
        }

        public async Task SetStatusAsync(int id, string status)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync("UPDATE Rooms SET Status=@status WHERE Id=@Id;", new { Id = id, status });
        }

        public async Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"SELECT COUNT(1) FROM Rooms WHERE RoomNumber=@roomNumber AND (@excludeId IS NULL OR Id <> @excludeId);";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { roomNumber, excludeId });
            return count > 0;
        }
    }
}

