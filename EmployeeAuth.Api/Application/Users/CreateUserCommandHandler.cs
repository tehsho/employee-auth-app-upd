using EmployeeAuth.Domain.Entities;
using EmployeeAuth.Infrastructure.Email;
using EmployeeAuth.Infrastructure.Persistence;
using EmployeeAuth.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAuth.Application.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IPasswordPolicyValidator _policyValidator;
    private readonly IPasswordHasherService _hasher;
    private readonly IEmailSender _email;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        AppDbContext db,
        IPasswordGenerator passwordGenerator,
        IPasswordPolicyValidator policyValidator,
        IPasswordHasherService hasher,
        IEmailSender email,
        ILogger<CreateUserCommandHandler> logger)
    {
        _db = db;
        _passwordGenerator = passwordGenerator;
        _policyValidator = policyValidator;
        _hasher = hasher;
        _email = email;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Creating user {UserName} ({Email})", request.UserName, request.Email);

        // Uniqueness checks
        if (await _db.Users.AnyAsync(u => u.UserName == request.UserName, ct))
            throw new InvalidOperationException("UserName already exists.");

        if (await _db.Users.AnyAsync(u => u.Email == request.Email, ct))
            throw new InvalidOperationException("Email already exists.");

        // Generate password that matches policy
        string password;
        int attempts = 0;
        do
        {
            if (++attempts > 20) throw new InvalidOperationException("Failed to generate a valid password.");
            password = _passwordGenerator.Generate();
        }
        while (!_policyValidator.IsValid(password, out _));

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName.Trim(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = _hasher.Hash(user, password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("User created {UserId}. Sending email...", user.Id);

        await _email.SendAsync(
            user.Email,
            "Your account password",
            $"Hello {user.Name},\n\nYour account has been created.\nYour temporary password is:\n\n{password}\n\nPlease log in and change it.\n"
        );
        _logger.LogInformation("Email sent to {Email}", request.Email);

        return user.Id;
    }
}
