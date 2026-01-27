using EmployeeAuth.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmployeeAuth.Infrastructure.Security;

public interface IPasswordHasherService
{
    string Hash(User user, string password);
    bool Verify(User user, string password);
}

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(User user, string password)
        => _hasher.HashPassword(user, password);

    public bool Verify(User user, string password)
        => _hasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success;
}
