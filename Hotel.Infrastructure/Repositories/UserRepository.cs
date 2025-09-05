using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;

namespace Hotel.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnectionFactory _factory;
        public UserRepository(SqlConnectionFactory factory) => _factory = factory;

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT TOP 1 * FROM Users WHERE Username=@Username AND ActiveFlag=1",
                new { Username = username });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT TOP 1 * FROM Users WHERE Id=@Id",
                new { Id = id });
        }

        public async Task<IEnumerable<User>> GetAllActiveAsync(int page = 1, int pageSize = 50, string? q = null)
        {
            using var conn = _factory.CreateConnection();
            var offset = (page - 1) * pageSize;
            var like = string.IsNullOrWhiteSpace(q) ? null : $"%{q}%";
            var sql = @"
SELECT * FROM Users
WHERE ActiveFlag=1 AND (@q IS NULL OR Username LIKE @like)
ORDER BY Id DESC
OFFSET @offset ROWS FETCH NEXT @ps ROWS ONLY;";
            return await conn.QueryAsync<User>(sql, new { q, like, offset, ps = pageSize });
        }

        public async Task<int> CreateAsync(User user)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"
INSERT INTO Users(Username, PasswordHash, Role, ActiveFlag)
VALUES(@Username, @PasswordHash, @Role, @ActiveFlag);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await conn.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task UpdateAsync(User user)
        {
            using var conn = _factory.CreateConnection();
            var sql = @"UPDATE Users SET Username=@Username, Role=@Role, ActiveFlag=@ActiveFlag WHERE Id=@Id;";
            await conn.ExecuteAsync(sql, user);
        }

        public async Task DeactivateAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync("UPDATE Users SET ActiveFlag=0 WHERE Id=@Id;", new { Id = id });
        }

        public async Task UpdatePasswordHashAsync(int id, string newHash)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE Users SET PasswordHash=@newHash WHERE Id=@Id;",
                new { Id = id, newHash });
        }

        public async Task UpdateRoleAsync(int id, string role)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE Users SET Role=@role WHERE Id=@Id;",
                new { Id = id, role });
        }

        public async Task SetActiveAsync(int id, bool active)
        {
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE Users SET ActiveFlag=@active WHERE Id=@Id;",
                new { Id = id, active });
        }
    }
}
