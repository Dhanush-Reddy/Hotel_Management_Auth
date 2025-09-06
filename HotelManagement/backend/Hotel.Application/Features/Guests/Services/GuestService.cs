using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Application.Features.Guests.Interfaces;
using Hotel.Domain.Features.Guests.Entities;

namespace Hotel.Application.Features.Guests.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _repo;
        public GuestService(IGuestRepository repo) => _repo = repo;

        public async Task<int> CreateAsync(string fullName, string? phone, string? email, string? idProof)
        {
            if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Full name required");

            // ✅ Check by email
            if (!string.IsNullOrWhiteSpace(email))
            {
                var existingByEmail = await _repo.FindByEmailAsync(email.Trim());
                if (existingByEmail != null) return existingByEmail.Id;
            }

            // ✅ Check by phone
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var existingByPhone = await _repo.FindByPhoneAsync(phone.Trim());
                if (existingByPhone != null) return existingByPhone.Id;
            }

            // Create new guest if not found
            var g = new Guest {
                FullName = fullName.Trim(),
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                IdProof = string.IsNullOrWhiteSpace(idProof) ? null : idProof.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            return await _repo.CreateAsync(g);
        }

        public Task<Guest?> GetAsync(int id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Guest>> SearchAsync(int page = 1, int pageSize = 50, string? q = null)
            => _repo.SearchAsync(page, pageSize, q);

        public async Task UpdateAsync(int id, string? fullName, string? phone, string? email, string? idProof)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Guest not found");
            if (!string.IsNullOrWhiteSpace(fullName)) existing.FullName = fullName.Trim();
            existing.Phone  = string.IsNullOrWhiteSpace(phone) ? existing.Phone : phone.Trim();
            existing.Email  = string.IsNullOrWhiteSpace(email) ? existing.Email : email.Trim();
            existing.IdProof= string.IsNullOrWhiteSpace(idProof)? existing.IdProof : idProof.Trim();
            await _repo.UpdateAsync(existing);
        }
    }
}
