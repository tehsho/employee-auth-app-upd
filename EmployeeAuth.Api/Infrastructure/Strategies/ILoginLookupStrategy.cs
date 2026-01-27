using EmployeeAuth.Domain.Entities;
using EmployeeAuth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAuth.Infrastructure.Strategies;

public interface ILoginLookupStrategy
{
    Task<User?> FindUserAsync(AppDbContext db, string login);
}

public class UsernameLookupStrategy : ILoginLookupStrategy
{
    public Task<User?> FindUserAsync(AppDbContext db, string login)
        => db.Users.FirstOrDefaultAsync(u => u.UserName == login);
}

public class EmailLookupStrategy : ILoginLookupStrategy
{
    public Task<User?> FindUserAsync(AppDbContext db, string login)
        => db.Users.FirstOrDefaultAsync(u => u.Email == login);
}

public interface ILoginLookupStrategyResolver
{
    ILoginLookupStrategy Resolve(string login);
}

public class LoginLookupStrategyResolver : ILoginLookupStrategyResolver
{
    private readonly UsernameLookupStrategy _username;
    private readonly EmailLookupStrategy _email;

    public LoginLookupStrategyResolver(UsernameLookupStrategy username, EmailLookupStrategy email)
    {
        _username = username;
        _email = email;
    }

    public ILoginLookupStrategy Resolve(string login)
        => login.Contains('@') ? _email : _username;
}
