using EmployeeAuth.Domain.Options;
using Microsoft.Extensions.Options;

namespace EmployeeAuth.Infrastructure.Security;

public interface IPasswordPolicyValidator
{
    bool IsValid(string password, out string error);
}

public class PasswordPolicyValidator : IPasswordPolicyValidator
{
    private readonly PasswordPolicyOptions _opt;

    public PasswordPolicyValidator(IOptions<PasswordPolicyOptions> options)
    {
        _opt = options.Value;
    }

    public bool IsValid(string password, out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(password))
        {
            error = "Password is required.";
            return false;
        }

        if (password.Length < _opt.MinLength)
        {
            error = $"Password must be at least {_opt.MinLength} characters long.";
            return false;
        }

        var allowed = _opt.AllowedSpecialChars ?? "";
        int specialCount = password.Count(c => allowed.Contains(c));

        if (specialCount < _opt.MinSpecialChars)
        {
            error = $"Password must contain at least {_opt.MinSpecialChars} special character(s) from: {allowed}";
            return false;
        }

        return true;
    }
}
