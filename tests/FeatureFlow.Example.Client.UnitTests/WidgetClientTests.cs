using System.Net;
using FeatureFlow.Example.Client;
using Xunit;

namespace FeatureFlow.Example.Client.UnitTests;

[Trait("Category", "unit")]
public sealed class WidgetClientTests
{
    [Fact]
    public async Task List_gets_widgets_and_reads_the_api_json_shape()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, """[{"id":1,"name":"alpha"},{"id":2,"name":"beta"}]""");

        var widgets = await handler.CreateClient().ListAsync();

        Assert.Equal(HttpMethod.Get, handler.Request!.Method);
        Assert.Equal("http://example.test/widgets", handler.Request.RequestUri!.ToString());
        Assert.Equal([new Widget(1, "alpha"), new Widget(2, "beta")], widgets);
    }

    [Fact]
    public async Task Get_returns_the_widget()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, """{"id":7,"name":"gamma"}""");

        var widget = await handler.CreateClient().GetAsync(7);

        Assert.Equal("http://example.test/widgets/7", handler.Request!.RequestUri!.ToString());
        Assert.Equal(new Widget(7, "gamma"), widget);
    }

    [Fact]
    public async Task Get_returns_null_for_not_found()
    {
        var handler = new FakeHandler(HttpStatusCode.NotFound);

        Assert.Null(await handler.CreateClient().GetAsync(404));
    }

    [Fact]
    public async Task Add_posts_the_name_and_returns_the_created_widget()
    {
        var handler = new FakeHandler(HttpStatusCode.Created, """{"id":3,"name":"delta"}""");

        var widget = await handler.CreateClient().AddAsync("delta");

        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal("http://example.test/widgets", handler.Request.RequestUri!.ToString());
        Assert.Equal("""{"name":"delta"}""", handler.RequestBody);
        Assert.Equal(new Widget(3, "delta"), widget);
    }

    [Fact]
    public async Task Add_rejects_a_blank_name_without_calling_the_api()
    {
        var handler = new FakeHandler(HttpStatusCode.Created);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.CreateClient().AddAsync("  "));
        Assert.Null(handler.Request);
    }

    [Fact]
    public async Task Add_throws_when_the_api_rejects_the_request()
    {
        var handler = new FakeHandler(HttpStatusCode.BadRequest, """{"error":"name is required"}""");

        await Assert.ThrowsAsync<HttpRequestException>(() => handler.CreateClient().AddAsync("x"));
    }

    [Fact]
    public async Task Count_sends_a_get_to_the_count_route()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, """{"count":5}""");

        await handler.CreateClient().CountAsync();

        Assert.Equal(HttpMethod.Get, handler.Request!.Method);
        Assert.Equal("http://example.test/widgets/count", handler.Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task Count_returns_the_reported_count()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, """{"count":5}""");

        Assert.Equal(5, await handler.CreateClient().CountAsync());
    }

    [Fact]
    public async Task Count_returns_zero_for_an_empty_store()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, """{"count":0}""");

        Assert.Equal(0, await handler.CreateClient().CountAsync());
    }

    [Fact]
    public async Task Count_throws_http_exception_when_the_api_rejects_the_request()
    {
        var handler = new FakeHandler(HttpStatusCode.NotFound);

        await Assert.ThrowsAsync<HttpRequestException>(() => handler.CreateClient().CountAsync());
    }

    [Fact]
    public async Task Count_throws_invalid_operation_for_an_empty_body()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "");

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.CreateClient().CountAsync());
    }

    [Fact]
    public async Task Count_throws_invalid_operation_for_a_null_body()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "null");

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.CreateClient().CountAsync());
    }

    [Fact]
    public async Task Count_throws_operation_canceled_when_the_call_is_cancelled()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, """{"count":5}""");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // HttpClient may surface a cancelled call as TaskCanceledException, a subclass, so accept the base type.
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.CreateClient().CountAsync(cts.Token));
    }
}
