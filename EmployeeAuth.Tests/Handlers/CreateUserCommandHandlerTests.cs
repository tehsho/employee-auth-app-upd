using EmployeeAuth.Application.Users;
using EmployeeAuth.Domain.Entities;
using EmployeeAuth.Infrastructure.Email;
using EmployeeAuth.Infrastructure.Security;
using EmployeeAuth.Tests.TestUtils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeAuth.Tests.Handlers;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Creates_user_generates_password_hash_and_sends_email()
    {
        // Arrange (KEEP CONNECTION OPEN!)
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        var generator = new Mock<IPasswordGenerator>(MockBehavior.Strict);
        var policy = new Mock<IPasswordPolicyValidator>(MockBehavior.Strict);

        var logger = new Mock<ILogger<CreateUserCommandHandler>>();

        const string generatedPassword = "GenPass#123!";
        const string hashedPassword = "HASHED";

        generator.Setup(g => g.Generate()).Returns(generatedPassword);

        policy
            .Setup(p => p.IsValid(generatedPassword, out It.Ref<string>.IsAny))
            .Returns(true);

        hasher
            .Setup(h => h.Hash(It.IsAny<User>(), generatedPassword))
            .Returns(hashedPassword);

        emailSender
            .Setup(e => e.SendAsync(
                "john@test.com",
                It.IsAny<string>(),
                It.Is<string>(b => b.Contains(generatedPassword))))
            .Returns(Task.CompletedTask);

        var handler = new CreateUserCommandHandler(
            db,
            generator.Object,
            policy.Object,
            hasher.Object,
            emailSender.Object,
            logger.Object);

        var cmd = new CreateUserCommand("john", "John Doe", "john@test.com");

        // Act
        var userId = await handler.Handle(cmd, default);

        // Assert
        userId.Should().NotBeEmpty();

        var user = await db.Users.SingleAsync(u => u.Id == userId);
        user.UserName.Should().Be("john");
        user.Email.Should().Be("john@test.com");
        user.Name.Should().Be("John Doe");
        user.PasswordHash.Should().Be(hashedPassword);

        generator.VerifyAll();
        policy.VerifyAll();
        hasher.VerifyAll();
        emailSender.VerifyAll();
    }

    [Fact]
    public async Task Rejects_duplicate_username_or_email()
    {
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Name = "Existing",
            Email = "john@test.com",
            PasswordHash = "x",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new CreateUserCommandHandler(
            db,
            Mock.Of<IPasswordGenerator>(),
            Mock.Of<IPasswordPolicyValidator>(),
            Mock.Of<IPasswordHasherService>(),
            Mock.Of<IEmailSender>(),
            Mock.Of<ILogger<CreateUserCommandHandler>>());

        Func<Task> sameUsername =
            async () => await handler.Handle(new CreateUserCommand("john", "A", "a@test.com"), default);

        Func<Task> sameEmail =
            async () => await handler.Handle(new CreateUserCommand("x", "A", "john@test.com"), default);

        await sameUsername.Should().ThrowAsync<Exception>();
        await sameEmail.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Rejects_when_generated_password_fails_policy()
    {
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        var generator = new Mock<IPasswordGenerator>(MockBehavior.Strict);
        var policy = new Mock<IPasswordPolicyValidator>(MockBehavior.Strict);

        var logger = new Mock<ILogger<CreateUserCommandHandler>>();

        const string generatedPassword = "bad";
        generator.Setup(g => g.Generate()).Returns(generatedPassword);

        var policyError = "Password too weak";
        policy
            .Setup(p => p.IsValid(generatedPassword, out policyError))
            .Returns(false);

        var handler = new CreateUserCommandHandler(
            db,
            generator.Object,
            policy.Object,
            hasher.Object,
            emailSender.Object,
            logger.Object);

        Func<Task> act =
            async () => await handler.Handle(
                new CreateUserCommand("john", "John Doe", "john@test.com"),
                default);

        await act.Should().ThrowAsync<Exception>();

        hasher.Verify(h => h.Hash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        emailSender.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        generator.VerifyAll();
        policy.VerifyAll();
    }
}
