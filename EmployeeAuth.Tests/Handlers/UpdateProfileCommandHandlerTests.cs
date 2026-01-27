using EmployeeAuth.Application.Users;
using EmployeeAuth.Domain.Entities;
using EmployeeAuth.Infrastructure.Security;
using EmployeeAuth.Tests.TestUtils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeAuth.Tests.Handlers;

public class UpdateProfileCommandHandlerTests
{
    [Fact]
    public async Task Updates_name_only_when_no_new_password()
    {
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Name = "John Old",
            Email = "john@test.com",
            PasswordHash = "HASHED_OLD",
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            UpdatedAtUtc = DateTime.UtcNow.AddDays(-1)
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var policy = new Mock<IPasswordPolicyValidator>(MockBehavior.Strict);
        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        var logger = new Mock<ILogger<UpdateProfileCommandHandler>>();

        var handler = new UpdateProfileCommandHandler(db, policy.Object, hasher.Object, logger.Object);

        var before = DateTime.UtcNow; // capture just before Act

        var cmd = new UpdateProfileCommand(user.Id, "  John New  ", null);

        // Act
        await handler.Handle(cmd, default);

        // Assert
        var updated = await db.Users.AsNoTracking().SingleAsync(u => u.Id == user.Id);
        updated.Name.Should().Be("John New");
        updated.PasswordHash.Should().Be("HASHED_OLD");
        updated.UpdatedAtUtc.Should().BeOnOrAfter(before);

        policy.Verify(p => p.IsValid(It.IsAny<string>(), out It.Ref<string>.IsAny), Times.Never);
        hasher.Verify(h => h.Hash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task Updates_password_when_policy_valid()
    {
        // Arrange (KEEP CONNECTION OPEN!)
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Name = "John Old",
            Email = "john@test.com",
            PasswordHash = "HASHED_OLD",
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            UpdatedAtUtc = DateTime.UtcNow.AddDays(-1)
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var policy = new Mock<IPasswordPolicyValidator>(MockBehavior.Strict);
        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        var logger = new Mock<ILogger<UpdateProfileCommandHandler>>();

        const string newPassword = "NewPass#2026!";
        const string newHash = "HASHED_NEW";

        policy
            .Setup(p => p.IsValid(newPassword, out It.Ref<string>.IsAny))
            .Returns(true);

        hasher
            .Setup(h => h.Hash(It.Is<User>(u => u.Id == user.Id), newPassword))
            .Returns(newHash);

        var handler = new UpdateProfileCommandHandler(
            db,
            policy.Object,
            hasher.Object,
            logger.Object);

        var cmd = new UpdateProfileCommand(user.Id, "John New", newPassword);

        // Act
        await handler.Handle(cmd, default);

        // Assert
        var updated = await db.Users.SingleAsync(u => u.Id == user.Id);
        updated.Name.Should().Be("John New");
        updated.PasswordHash.Should().Be(newHash);

        policy.VerifyAll();
        hasher.VerifyAll();
    }

    [Fact]
    public async Task Rejects_when_user_not_found()
    {
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var handler = new UpdateProfileCommandHandler(
            db,
            Mock.Of<IPasswordPolicyValidator>(),
            Mock.Of<IPasswordHasherService>(),
            Mock.Of<ILogger<UpdateProfileCommandHandler>>());

        Func<Task> act = async () =>
            await handler.Handle(new UpdateProfileCommand(Guid.NewGuid(), "Name", null), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task Rejects_when_new_password_fails_policy_and_does_not_hash()
    {
        var (db, conn) = DbFactory.CreateSqliteInMemoryDb();
        await using var _db = db;
        await using var _conn = conn;

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "john",
            Name = "John Old",
            Email = "john@test.com",
            PasswordHash = "HASHED_OLD",
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            UpdatedAtUtc = DateTime.UtcNow.AddDays(-1)
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var policy = new Mock<IPasswordPolicyValidator>(MockBehavior.Strict);
        var hasher = new Mock<IPasswordHasherService>(MockBehavior.Strict);
        var logger = new Mock<ILogger<UpdateProfileCommandHandler>>();

        const string newPassword = "bad";
        var error = "Password too weak";

        // Policy rejects
        policy.Setup(p => p.IsValid(newPassword, out error)).Returns(false);

        var handler = new UpdateProfileCommandHandler(
            db,
            policy.Object,
            hasher.Object,
            logger.Object);

        Func<Task> act = async () =>
            await handler.Handle(new UpdateProfileCommand(user.Id, "John New", newPassword), default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(error);

        // Make sure no hashing happened
        hasher.Verify(h => h.Hash(It.IsAny<User>(), It.IsAny<string>()), Times.Never);

        // DB should not have changed password
        var unchanged = await db.Users.SingleAsync(u => u.Id == user.Id);
        unchanged.PasswordHash.Should().Be("HASHED_OLD");

        policy.VerifyAll();
    }
}
