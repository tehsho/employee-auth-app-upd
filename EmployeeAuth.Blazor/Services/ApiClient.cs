using System.Net;
using System.Net.Http.Headers;

namespace EmployeeAuth.Blazor.Services;
public sealed class ApiError
{
    public string? Error { get; set; }
}

public sealed class ApiClient
{
    private readonly IHttpClientFactory _httpFactory;

    public ApiClient(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    private HttpClient Http => _httpFactory.CreateClient("Api");

    public async Task<string> LoginAsync(string login, string password, CancellationToken ct = default)
    {
        var res = await Http.PostAsJsonAsync("/api/auth/login", new LoginRequest(login, password), ct);
        await EnsureSuccessOrThrow(res);

        var body = await res.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
        if (body is null || string.IsNullOrWhiteSpace(body.Token))
            throw new Exception("Login response did not contain a token.");

        return body.Token;
    }

    public async Task<Guid> RegisterAsync(string userName, string name, string email, CancellationToken ct = default)
    {
        var res = await Http.PostAsJsonAsync("/api/users", new CreateUserRequest(userName, name, email), ct);
        await EnsureSuccessOrThrow(res);

        // Your API returns 201 + { id: "..." }
        var body = await res.Content.ReadFromJsonAsync<CreateUserResponse>(cancellationToken: ct);
        if (body is null || body.Id == Guid.Empty)
            throw new Exception("Registration response did not contain a valid id.");

        return body.Id;
    }

    public async Task<MeResponse> GetMeAsync(string token, CancellationToken ct = default)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await Http.SendAsync(req, ct);
        await EnsureSuccessOrThrow(res);

        var body = await res.Content.ReadFromJsonAsync<MeResponse>(cancellationToken: ct);
        if (body is null)
            throw new Exception("Failed to load profile.");

        return body;
    }

    public async Task UpdateMeAsync(string token, string name, string? newPassword, CancellationToken ct = default)
    {
        var req = new HttpRequestMessage(HttpMethod.Put, "/api/users/me");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Content = JsonContent.Create(new UpdateMeRequest(name, newPassword));

        var res = await Http.SendAsync(req, ct);
        await EnsureSuccessOrThrow(res);
    }

    private static async Task EnsureSuccessOrThrow(HttpResponseMessage res)
    {
        if (res.IsSuccessStatusCode) return;

        var contentType = res.Content.Headers.ContentType?.MediaType ?? "";
        var body = await res.Content.ReadAsStringAsync();

        // Try to extract {"error":"..."} nicely
        string message = "Request failed.";

        if (contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var apiErr = System.Text.Json.JsonSerializer.Deserialize<ApiError>(
                    body,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (!string.IsNullOrWhiteSpace(apiErr?.Error))
                    message = apiErr.Error!;
                else if (!string.IsNullOrWhiteSpace(body))
                    message = body;
            }
            catch
            {
                // Not valid JSON; fall back to body
                if (!string.IsNullOrWhiteSpace(body))
                    message = body;
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(body))
                message = body;
        }

        if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(message);

        throw new Exception(message);
    }

    // DTOs matching your API JSON
    private sealed record LoginRequest(string Login, string Password);
    private sealed record LoginResponse(string Token);

    private sealed record CreateUserRequest(string UserName, string Name, string Email);
    private sealed record CreateUserResponse(Guid Id);

    public sealed record MeResponse(string UserName, string Name, string Email);

    private sealed record UpdateMeRequest(string Name, string? NewPassword);
}
