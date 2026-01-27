using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace EmployeeAuth.Tests.Integration;

public class InvalidLoginIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InvalidLoginIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            login = "nosuchuser",
            password = "bad"
        });

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
