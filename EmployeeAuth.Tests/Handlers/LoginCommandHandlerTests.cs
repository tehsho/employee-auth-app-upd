using EmployeeAuth.Domain.Entities;
using EmployeeAuth.Infrastructure.Security;
using EmployeeAuth.Infrastructure.Strategies;
using EmployeeAuth.Tests.TestUtils;
using FluentAssertions;
using Moq;

using EmployeeAuth.Application.Auth;

namespace EmployeeAuth.Tests.Handlers;

public class LoginCommandHandlerTests
{
    private static ILoginLookupStrategyResolver MakeResolver()
        => new LoginLookupStrategyResolver(new UsernameLookupStrategy(), new EmailLookupStrategy());

    [Fact]
    public async Task Login_with_username_returns_token()
    {
        // Arrange (lets keep connection open)
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Name = "John",
            Email = "john@test.com",
            PasswordHash = "HASH",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var resolver = MakeResolver();

        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        hasher.Setup(h => h.Verify(It.Is<User>(u => u.UserName == "john"), "pw")).Returns(true);

        var jwt = new Mock<IJwtTokenService>(MockBehavior.Strict);
        jwt.Setup(j => j.CreateToken(It.Is<User>(u => u.UserName == "john"))).Returns("TOKEN");

        // Order is important: db, resolver, hasher, jwt
        var handler = new LoginCommandHandler(db, resolver, hasher.Object, jwt.Object);

        // Act
        var token = await handler.Handle(new LoginCommand("john", "pw"), default);

        // Assert
        token.Should().Be("TOKEN");
        hasher.VerifyAll();
        jwt.VerifyAll();
    }

    [Fact]
    public async Task Login_with_email_returns_token()
    {
        // Arrange (lets keep connection open)
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Name = "John",
            Email = "john@test.com",
            PasswordHash = "HASH",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var resolver = MakeResolver();

        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        hasher.Setup(h => h.Verify(It.Is<User>(u => u.Email == "john@test.com"), "pw")).Returns(true);

        var jwt = new Mock<IJwtTokenService>(MockBehavior.Strict);
        jwt.Setup(j => j.CreateToken(It.Is<User>(u => u.Email == "john@test.com"))).Returns("TOKEN");

        var handler = new LoginCommandHandler(db, resolver, hasher.Object, jwt.Object);

        // Act
        var token = await handler.Handle(new LoginCommand("john@test.com", "pw"), default);

        // Assert
        token.Should().Be("TOKEN");
        hasher.VerifyAll();
        jwt.VerifyAll();
    }

    [Fact]
    public async Task Unknown_user_throws_unauthorized()
    {
        // Arrange (lets keep connection open)
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var resolver = MakeResolver();

        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        var jwt = new Mock<IJwtTokenService>(MockBehavior.Strict);

        var handler = new LoginCommandHandler(db, resolver, hasher.Object, jwt.Object);

        // Act + Assert
        Func<Task> act = async () => await handler.Handle(new LoginCommand("missing", "pw"), default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();

        // No verification/token should happen
        hasher.Verify(h => h.Verify(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        jwt.Verify(j => j.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Wrong_password_throws_unauthorized()
    {
        // Arrange (lets keep connection open)
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Name = "John",
            Email = "john@test.com",
            PasswordHash = "HASH",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var resolver = MakeResolver();

        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        hasher.Setup(h => h.Verify(It.Is<User>(u => u.UserName == "john"), "wrong")).Returns(false);

        var jwt = new Mock<IJwtTokenService>(MockBehavior.Strict);

        var handler = new LoginCommandHandler(db, resolver, hasher.Object, jwt.Object);

        // Act + Assert
        Func<Task> act = async () => await handler.Handle(new LoginCommand("john", "wrong"), default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();

        // Token should NOT be created
        jwt.Verify(j => j.CreateToken(It.IsAny<User>()), Times.Never);

        hasher.VerifyAll();
    }
}
