using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Application.Features.Rooms.Interfaces;
using Hotel.Domain.Features.Rooms.Entities;

namespace Hotel.Application.Features.Rooms.Services
{
    public class RoomService : IRoomService
    {
        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        { "Available", "Occupied", "OutOfService" };

        private readonly IRoomRepository _repo;

        public RoomService(IRoomRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Room>> ListAsync(int page = 1, int pageSize = 50, string? q = null, string? status = null)
            => _repo.ListAsync(page, pageSize, q, status);

        public Task<Room?> GetAsync(int id) => _repo.GetByIdAsync(id);

        public async Task<int> CreateAsync(string roomNumber, int capacity)
        {
            if (string.IsNullOrWhiteSpace(roomNumber)) throw new ArgumentException("Room number required");
            if (capacity <= 0) throw new ArgumentException("Capacity must be > 0");
            if (await _repo.RoomNumberExistsAsync(roomNumber, null)) throw new InvalidOperationException("Room number already exists");

            var room = new Room { RoomNumber = roomNumber.Trim(), Capacity = capacity, Status = "Available" };
            return await _repo.CreateAsync(room);
        }

        public async Task UpdateAsync(int id, string roomNumber, int capacity)
        {
            if (string.IsNullOrWhiteSpace(roomNumber)) throw new ArgumentException("Room number required");
            if (capacity <= 0) throw new ArgumentException("Capacity must be > 0");
            if (await _repo.RoomNumberExistsAsync(roomNumber, id)) throw new InvalidOperationException("Room number already exists");

            var existing = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Room not found");
            existing.RoomNumber = roomNumber.Trim();
            existing.Capacity = capacity;
            await _repo.UpdateAsync(existing);
        }

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

        public async Task SetStatusAsync(int id, string status)
        {
            if (!AllowedStatuses.Contains(status ?? "")) throw new ArgumentException("Invalid status");
            await _repo.SetStatusAsync(id, status);
        }
    }
}

