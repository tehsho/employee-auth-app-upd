using EmployeeAuth.Domain.Options;
using EmployeeAuth.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmployeeAuth.Tests.Security;

public class AuthFlowIntegrationTests
{
    private static PasswordPolicyValidator Create(int minLength, int minSpecial)
    {
        var options = Options.Create(new PasswordPolicyOptions
        {
            MinLength = minLength,
            MinSpecialChars = minSpecial
        });

        return new PasswordPolicyValidator(options);
    }

    [Fact]
    public void Rejects_when_too_short()
    {
        var validator = Create(minLength: 10, minSpecial: 1);

        var ok = validator.IsValid("Abc#1", out var error);

        ok.Should().BeFalse();
        error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Rejects_when_not_enough_special_chars()
    {
        var validator = Create(minLength: 8, minSpecial: 2);

        // only one special char
        var ok = validator.IsValid("Abcdefg#1", out var error);

        ok.Should().BeFalse();
        error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Accepts_valid_password()
    {
        var validator = Create(minLength: 8, minSpecial: 2);

        // two special chars: # and !
        var ok = validator.IsValid("Abcd#2026!", out var error);

        ok.Should().BeTrue();
        error.Should().BeNullOrEmpty();
    }
}
