using Hotel.Domain.Features.Users.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.Application.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 50, string? q = null);
        Task<int> CreateUserAsync(string username, string role, string password);
        Task UpdateRoleAsync(int id, string role);
        Task SetActiveAsync(int id, bool active);
        Task ResetPasswordAsync(int id, string newPassword);
        Task<User?> GetByIdAsync(int id);
    }
}
