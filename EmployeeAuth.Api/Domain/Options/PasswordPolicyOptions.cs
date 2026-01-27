namespace EmployeeAuth.Domain.Options;

public class PasswordPolicyOptions
{
    public int MinLength { get; set; } = 12;
    public int MinSpecialChars { get; set; } = 2;
    public string AllowedSpecialChars { get; set; } = "@#!%&";
}
