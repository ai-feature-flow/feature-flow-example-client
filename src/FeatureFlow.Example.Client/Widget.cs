namespace FeatureFlow.Example.Client;

/// <summary>A widget as returned by feature-flow-example: <c>{ "id": 1, "name": "alpha" }</c>.</summary>
public sealed record Widget(int Id, string Name);

/// <summary>The body of <c>POST /widgets</c>: <c>{ "name": "alpha" }</c>.</summary>
public sealed record WidgetCreateRequest(string Name);

/// <summary>
/// The body of <c>GET /widgets/count</c>: <c>{ "count": n }</c>. <see cref="Count"/> is nullable so that
/// a body without a <c>count</c> property is distinguishable from <c>count: 0</c>.
/// </summary>
internal sealed record WidgetCountResponse(int? Count);
