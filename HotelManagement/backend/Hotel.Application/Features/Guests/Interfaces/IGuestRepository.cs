using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Guests.Entities;

namespace Hotel.Application.Features.Guests.Interfaces
{
    public interface IGuestRepository
    {
        Task<int> CreateAsync(Guest guest);
        Task<Guest?> GetByIdAsync(int id);
        Task<Guest?> FindByEmailAsync(string email);
        Task<Guest?> FindByPhoneAsync(string phone);
        Task<IEnumerable<Guest>> SearchAsync(int page = 1, int pageSize = 50, string? q = null);
        Task UpdateAsync(Guest guest);
        Task DeleteAsync(int id);
    }
}
