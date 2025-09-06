using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Hotel.Application.Features.Guests.Interfaces;
using Hotel.Domain.Features.Guests.Entities;
using Hotel.Infrastructure.Common.Data;

namespace Hotel.Infrastructure.Features.Guests.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly SqlConnectionFactory _factory;
        public GuestRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Guest guest)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
INSERT INTO Guests(FullName, Phone, Email, IdProof, CreatedAt)
VALUES(@FullName, @Phone, @Email, @IdProof, SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await conn.ExecuteScalarAsync<int>(sql, guest);
        }

        public async Task<Guest?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Guest>(
                "SELECT TOP 1 * FROM Guests WHERE Id=@Id", new { Id = id });
        }

        public async Task<Guest?> FindByEmailAsync(string email)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Guest>(
                "SELECT TOP 1 * FROM Guests WHERE Email=@Email", new { Email = email });
        }

        public async Task<Guest?> FindByPhoneAsync(string phone)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Guest>(
                "SELECT TOP 1 * FROM Guests WHERE Phone=@Phone", new { Phone = phone });
        }

        public async Task<IEnumerable<Guest>> SearchAsync(int page = 1, int pageSize = 50, string? q = null)
        {
            using var conn = _factory.CreateConnection();
            var offset = (page - 1) * pageSize;
            var like = string.IsNullOrWhiteSpace(q) ? null : $"%{q}%";
            var sql = @"
SELECT * FROM Guests
WHERE (@q IS NULL OR FullName LIKE @like OR Email LIKE @like OR Phone LIKE @like)
ORDER BY Id DESC
OFFSET @offset ROWS FETCH NEXT @ps ROWS ONLY;";
            return await conn.QueryAsync<Guest>(sql, new { q, like, offset, ps = pageSize });
        }

        public async Task UpdateAsync(Guest guest)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"UPDATE Guests SET FullName=@FullName, Phone=@Phone, Email=@Email, IdProof=@IdProof WHERE Id=@Id;";
            await conn.ExecuteAsync(sql, guest);
        }
    }
}
