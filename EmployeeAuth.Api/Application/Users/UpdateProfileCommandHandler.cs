using EmployeeAuth.Infrastructure.Persistence;
using EmployeeAuth.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAuth.Application.Users;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly AppDbContext _db;
    private readonly IPasswordPolicyValidator _policy;
    private readonly IPasswordHasherService _hasher;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(
        AppDbContext db,
        IPasswordPolicyValidator policy,
        IPasswordHasherService hasher,
        ILogger<UpdateProfileCommandHandler> logger)
    {
        _db = db;
        _policy = policy;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        _logger.LogInformation("UpdateProfile started for UserId = {UserId}", request.UserId);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
        if (user is null)
        {
            _logger.LogWarning("UpdateProfile failed: user not found. UserId = {UserId}", request.UserId);
            throw new InvalidOperationException("User not found.");
        }

        var newName = request.Name?.Trim() ?? "";
        var nameChanged = !string.Equals(user.Name, newName, StringComparison.Ordinal);

        user.Name = newName;

        var passwordChanged = false;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (!_policy.IsValid(request.NewPassword, out var error))
            {
                _logger.LogWarning("UpdateProfile rejected by password policy. UserId = {UserId}. Reason = {Reason}",
                    request.UserId, error);
                throw new InvalidOperationException(error);
            }

            user.PasswordHash = _hasher.Hash(user, request.NewPassword);
            passwordChanged = true;
        }

        user.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "UpdateProfile success for UserId={UserId}. NameChanged = {NameChanged}, PasswordChanged = {PasswordChanged}",
            request.UserId, nameChanged, passwordChanged);
    }
}
