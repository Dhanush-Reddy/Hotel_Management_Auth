using System.Threading.Tasks;
using System.Collections.Generic;
using Hotel.Application.Features.Users.Interfaces;
using Hotel.Application.Common.Interfaces;
using Hotel.Domain.Features.Users.Entities;

namespace Hotel.Application.Features.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;

        public UserService(IUserRepository repo, IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _repo.GetByUsernameAsync(username);
            if (user == null || !user.ActiveFlag) return null;
            if (!_hasher.VerifyPassword(password, user.PasswordHash)) return null;
            return user;
        }

        public Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50, string? q = null)
            => _repo.GetAllActiveAsync(page, pageSize, q);

        public async Task<int> CreateUserAsync(string username, string role, string password)
        {
            var hash = _hasher.HashPassword(password);
            var user = new User
            {
                Username = username,
                Role = role,
                PasswordHash = hash,
                ActiveFlag = true
            };
            return await _repo.CreateAsync(user);
        }

        public Task UpdateRoleAsync(int id, string role) => _repo.UpdateRoleAsync(id, role);
        public Task SetActiveAsync(int id, bool active) => _repo.SetActiveAsync(id, active);

        public async Task ResetPasswordAsync(int id, string newPassword)
        {
            var hash = _hasher.HashPassword(newPassword);
            await _repo.UpdatePasswordHashAsync(id, hash);
        }

        public Task<User?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    }
}
