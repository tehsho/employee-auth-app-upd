using System.Net.Http.Headers;

namespace EmployeeAuth.Blazor.Services;

public class JwtAuthHandler : DelegatingHandler
{
    private readonly TokenStore _tokenStore;

    public JwtAuthHandler(TokenStore tokenStore)
    {
        _tokenStore = tokenStore;
        Console.WriteLine("JWT handler attaching token? " + (!string.IsNullOrWhiteSpace(_tokenStore.Token)));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_tokenStore.Token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenStore.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
