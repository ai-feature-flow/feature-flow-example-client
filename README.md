# feature-flow-example-client

A fixture repository for [Feature Flow](https://github.com/ai-feature-flow/feature-flow-poc)'s live tests. It holds `WidgetClient`, a small .NET 9 typed `HttpClient` wrapper for the widget endpoints (`GET /widgets`, `GET /widgets/{id}`, `POST /widgets`) of [feature-flow-example](https://github.com/ai-feature-flow/feature-flow-example), with a fast unit tier (`dotnet test tests/FeatureFlow.Example.Client.UnitTests --filter Category=unit`) that fakes the HTTP handler and needs no network.

It exists so Feature Flow project runs can span two repositories: a change to the example API and the matching change to this client are planned, implemented and tested together. Its `feature-flow.yaml` declares a single `unit` tier, delegated to CI via `.github/workflows/unit.yml`.
