using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace EmployeeAuth.Tests.Integration;

public class AuthFailuresIntegrationTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AuthFailuresIntegrationTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_wrong_password_returns_401()
    {
        // first register
        await _client.PostAsJsonAsync("/api/users", new
        {
            userName = "u1",
            name = "U1",
            email = "u1@test.com"
        });

        var loginRes = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            login = "u1",
            password = "WRONG"
        });

        loginRes.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
