namespace FeatureFlow.Example.Client;

/// <summary>A widget as returned by feature-flow-example: <c>{ "id": 1, "name": "alpha" }</c>.</summary>
public sealed record Widget(int Id, string Name);

/// <summary>The body of <c>POST /widgets</c>: <c>{ "name": "alpha" }</c>.</summary>
public sealed record WidgetCreateRequest(string Name);
