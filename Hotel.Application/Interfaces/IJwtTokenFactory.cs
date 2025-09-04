using System;

namespace Hotel.Application.Interfaces
{
    public interface IJwtTokenFactory
    {
        (string Token, DateTime ExpiresAtUtc) CreateToken(int userId, string username, string role);
    }
}

