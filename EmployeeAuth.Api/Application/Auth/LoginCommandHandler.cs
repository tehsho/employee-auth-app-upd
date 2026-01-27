using EmployeeAuth.Infrastructure.Persistence;
using EmployeeAuth.Infrastructure.Security;
using EmployeeAuth.Infrastructure.Strategies;
using MediatR;

namespace EmployeeAuth.Application.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly AppDbContext _db;
    private readonly ILoginLookupStrategyResolver _resolver;
    private readonly IPasswordHasherService _hasher;
    private readonly IJwtTokenService _jwt;

    public LoginCommandHandler(
        AppDbContext db,
        ILoginLookupStrategyResolver resolver,
        IPasswordHasherService hasher,
        IJwtTokenService jwt)
    {
        _db = db;
        _resolver = resolver;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken ct)
    {
        var strategy = _resolver.Resolve(request.Login.Trim());
        var user = await strategy.FindUserAsync(_db, request.Login.Trim());

        // Avoid user enumeration: same error for not found/wrong password
        if (user is null) throw new UnauthorizedAccessException("Invalid credentials.");

        var ok = _hasher.Verify(user, request.Password);
        if (!ok) throw new UnauthorizedAccessException("Invalid credentials.");

        return _jwt.CreateToken(user);
    }
}
