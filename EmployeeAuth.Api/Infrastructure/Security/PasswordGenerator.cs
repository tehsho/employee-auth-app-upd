using System.Security.Cryptography;
using EmployeeAuth.Domain.Options;
using Microsoft.Extensions.Options;

namespace EmployeeAuth.Infrastructure.Security;

public interface IPasswordGenerator
{
    string Generate();
}

public class PasswordGenerator : IPasswordGenerator
{
    private readonly PasswordPolicyOptions _opt;
    private const string Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";

    public PasswordGenerator(IOptions<PasswordPolicyOptions> options)
    {
        _opt = options.Value;
    }

    public string Generate()
    {
        // Simple generator that guarantees policy:
        // - MinLength total
        // - MinSpecialChars from AllowedSpecialChars
        // - rest from letters+digits
        int minLen = Math.Max(_opt.MinLength, 6);
        int minSpecial = Math.Max(_opt.MinSpecialChars, 0);

        var specials = _opt.AllowedSpecialChars ?? "@#!%&";
        if (specials.Length == 0 && minSpecial > 0)
            throw new InvalidOperationException("AllowedSpecialChars is empty but MinSpecialChars > 0.");

        var chars = new List<char>();

        // Add required specials
        for (int i = 0; i < minSpecial; i++)
            chars.Add(RandomFrom(specials));

        // Fill remaining
        string pool = Letters + Digits;
        while (chars.Count < minLen)
            chars.Add(RandomFrom(pool));

        // Shuffle
        Shuffle(chars);
        return new string(chars.ToArray());
    }

    private static char RandomFrom(string pool)
    {
        int idx = RandomNumberGenerator.GetInt32(pool.Length);
        return pool[idx];
    }

    private static void Shuffle(List<char> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
