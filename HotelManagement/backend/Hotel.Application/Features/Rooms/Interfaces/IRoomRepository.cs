using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Features.Rooms.Entities;

namespace Hotel.Application.Features.Rooms.Interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> ListAsync(int page = 1, int pageSize = 50, string? q = null, string? status = null);
        Task<Room?> GetByIdAsync(int id);
        Task<int> CreateAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int id);
        Task SetStatusAsync(int id, string status);
        Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null);
    }
}

