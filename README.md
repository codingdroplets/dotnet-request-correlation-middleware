# dotnet-request-correlation-middleware

ASP.NET Core Web API example that implements request correlation with `X-Correlation-ID` for traceable debugging across services, using clean middleware design and Swagger-based testing.

![.NET](https://img.shields.io/badge/.NET-8-blueviolet)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![Last Commit](https://img.shields.io/badge/last%20commit-active-success)

## Key Features
- Adds correlation IDs for every incoming request.
- Reuses incoming `X-Correlation-ID` when available.
- Returns correlation ID in response headers for client-side tracing.
- Exposes a sample API endpoint with correlation ID in payload.
- Includes unit tests for middleware behavior.

## Architecture Overview
This sample uses an ASP.NET Core middleware-first architecture. The custom `CorrelationIdMiddleware` runs early in the pipeline, enriches `HttpContext`, and ensures response propagation for consistent request tracing.

## Tech Stack
- .NET 8 / ASP.NET Core Web API
- Swagger / OpenAPI
- xUnit for tests

## Getting Started
### Prerequisites
- .NET SDK 8+

### Run
```bash
cd CodingDroplets.RequestCorrelationMiddleware.Api
/home/ubuntu/.dotnet/dotnet run
```

### Test
```bash
/home/ubuntu/.dotnet/dotnet test ../CodingDroplets.RequestCorrelationMiddleware.sln
```

## Use Cases
- Debugging distributed request flows across microservices
- Correlating client errors with server logs
- Adding observability patterns to enterprise APIs

## Project Structure
```text
dotnet-request-correlation-middleware/
├── CodingDroplets.RequestCorrelationMiddleware.sln
├── CodingDroplets.RequestCorrelationMiddleware.Api/
│   ├── Program.cs
│   └── CodingDroplets.RequestCorrelationMiddleware.Api.csproj
├── CodingDroplets.RequestCorrelationMiddleware.Tests/
│   ├── CorrelationIdMiddlewareTests.cs
│   └── CodingDroplets.RequestCorrelationMiddleware.Tests.csproj
├── CHANGELOG.md
└── LICENSE
```

## Suggested GitHub Topics
`dotnet, csharp, aspnet-core, webapi, middleware, observability, request-tracing, distributed-systems, swagger, backend, starter-template, beginner-friendly`

## CHANGELOG
See [CHANGELOG.md](./CHANGELOG.md)

## License
This project is licensed under the MIT License - see [LICENSE](./LICENSE).

## Author / Maintainer
Coding Droplets
- Visit Now: https://codingdroplets.com
- Join our Patreon to Learn & Level Up: https://www.patreon.com/codingdroplets
