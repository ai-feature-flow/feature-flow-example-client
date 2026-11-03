using System.Net;
using System.Text;

namespace FeatureFlow.Example.Client.UnitTests;

/// <summary>Answers every request with a canned response and records what was sent. No network.</summary>
internal sealed class FakeHandler(HttpStatusCode status, string body = "") : HttpMessageHandler
{
    public HttpRequestMessage? Request { get; private set; }

    public string? RequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Request = request;
        RequestBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };
    }

    public WidgetClient CreateClient() =>
        new(new HttpClient(this) { BaseAddress = new Uri("http://example.test/") });
}
