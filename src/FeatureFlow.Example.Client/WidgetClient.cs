using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FeatureFlow.Example.Client;

/// <summary>
/// A typed <see cref="HttpClient"/> wrapper for feature-flow-example's widget endpoints.
/// The caller owns the <see cref="HttpClient"/> and sets its <see cref="HttpClient.BaseAddress"/>.
/// </summary>
public sealed class WidgetClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public WidgetClient(HttpClient http)
    {
        ArgumentNullException.ThrowIfNull(http);
        _http = http;
    }

    /// <summary><c>GET /widgets</c>: every widget, in the order the API returns them.</summary>
    public async Task<IReadOnlyList<Widget>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync("widgets", cancellationToken);
        response.EnsureSuccessStatusCode();
        var widgets = await response.Content.ReadFromJsonAsync<List<Widget>>(JsonOptions, cancellationToken);
        return widgets ?? [];
    }

    /// <summary><c>GET /widgets/{id}</c>: the widget, or null when the API answers 404.</summary>
    public async Task<Widget?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync($"widgets/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Widget>(JsonOptions, cancellationToken);
    }

    /// <summary><c>POST /widgets</c>: adds a widget and returns it with its assigned id.</summary>
    public async Task<Widget> AddAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        using var response = await _http.PostAsJsonAsync(
            "widgets", new WidgetCreateRequest(name), JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        var widget = await response.Content.ReadFromJsonAsync<Widget>(JsonOptions, cancellationToken);
        return widget ?? throw new InvalidOperationException("The API returned an empty widget.");
    }

    /// <summary><c>GET /widgets/count</c>: the number of widgets in the store. An empty store reports 0.</summary>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync("widgets/count", cancellationToken);
        response.EnsureSuccessStatusCode();
        var count = await response.Content.ReadFromJsonAsync<CountResponse>(JsonOptions, cancellationToken);
        return (count ?? throw new InvalidOperationException("The API returned an empty count.")).Count;
    }

    /// <summary>The body of <c>GET /widgets/count</c>: <c>{ "count": 5 }</c>. Internal to the client.</summary>
    private sealed record CountResponse(int Count);
}
