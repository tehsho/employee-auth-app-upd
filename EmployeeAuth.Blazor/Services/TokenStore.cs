namespace EmployeeAuth.Blazor.Services;

public class TokenStore
{
    public string? Token { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

    public void Set(string token) => Token = token;
    public void Clear() => Token = null;
}
