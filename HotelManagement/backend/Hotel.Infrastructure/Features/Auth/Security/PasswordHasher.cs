using BCrypt.Net;
using Hotel.Application.Common.Interfaces;

namespace Hotel.Infrastructure.Features.Auth.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public bool VerifyPassword(string password, string storedHash) => BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
