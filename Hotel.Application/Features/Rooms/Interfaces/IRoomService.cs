using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Rooms.Entities;

namespace Hotel.Application.Features.Rooms.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> ListAsync(int page = 1, int pageSize = 50, string? q = null, string? status = null);
        Task<Room?> GetAsync(int id);
        Task<int> CreateAsync(string roomNumber, int capacity);
        Task UpdateAsync(int id, string roomNumber, int capacity);
        Task DeleteAsync(int id);
        Task SetStatusAsync(int id, string status);
    }
}

