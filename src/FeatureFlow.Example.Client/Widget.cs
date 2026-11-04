using System.Text.Json.Serialization;

namespace FeatureFlow.Example.Client;

/// <summary>A widget as returned by feature-flow-example: <c>{ "id": 1, "name": "alpha" }</c>.</summary>
public sealed record Widget(int Id, string Name);

/// <summary>The body of <c>POST /widgets</c>: <c>{ "name": "alpha" }</c>.</summary>
public sealed record WidgetCreateRequest(string Name);

/// <summary>
/// The body of <c>GET /widgets/count</c>: <c>{ "count": 3 }</c>. Internal so the public surface
/// grows by one method only. <see cref="JsonRequiredAttribute"/> makes a body without <c>count</c>
/// fail deserialisation rather than silently producing 0.
/// </summary>
internal sealed record WidgetCountResponse([property: JsonRequired] int Count);
