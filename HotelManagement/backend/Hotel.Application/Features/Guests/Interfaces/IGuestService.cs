using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Guests.Entities;

namespace Hotel.Application.Features.Guests.Interfaces
{
    public interface IGuestService
    {
        Task<int> CreateAsync(string fullName, string? phone, string? email, string? idProof);
        Task<Guest?> GetAsync(int id);
        Task<IEnumerable<Guest>> SearchAsync(int page = 1, int pageSize = 50, string? q = null);
        Task UpdateAsync(int id, string? fullName, string? phone, string? email, string? idProof);
    }
}

