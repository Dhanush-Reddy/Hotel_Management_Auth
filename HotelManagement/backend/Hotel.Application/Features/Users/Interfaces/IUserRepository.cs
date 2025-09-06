using Hotel.Domain.Features.Users.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Application.Features.Users.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllActiveAsync(int page = 1, int pageSize = 50, string? q = null);
        Task<int> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeactivateAsync(int id);
        Task UpdatePasswordHashAsync(int id, string newHash);
        Task UpdateRoleAsync(int id, string role);
        Task SetActiveAsync(int id, bool active);
    }
}
