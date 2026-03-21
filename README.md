# Request Correlation Middleware in ASP.NET Core

> **Traceable APIs across microservices** — Custom middleware that stamps every request with an `X-Correlation-ID`, propagates it through the response, and makes distributed debugging actually possible.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4)](https://docs.microsoft.com/aspnet/core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Visit CodingDroplets](https://img.shields.io/badge/Website-codingdroplets.com-blue?style=flat&logo=google-chrome&logoColor=white)](https://codingdroplets.com/)
[![YouTube](https://img.shields.io/badge/YouTube-CodingDroplets-red?style=flat&logo=youtube&logoColor=white)](https://www.youtube.com/@CodingDroplets)
[![Patreon](https://img.shields.io/badge/Patreon-Support%20Us-orange?style=flat&logo=patreon&logoColor=white)](https://www.patreon.com/CodingDroplets)
[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-Support%20Us-yellow?style=flat&logo=buy-me-a-coffee&logoColor=black)](https://buymeacoffee.com/codingdroplets)
[![GitHub](https://img.shields.io/badge/GitHub-codingdroplets-black?style=flat&logo=github&logoColor=white)](http://github.com/codingdroplets/)

---

## 🚀 Support the Channel — Join on Patreon

If this sample saved you time, consider joining our Patreon community.
You'll get **exclusive .NET tutorials, premium code samples, and early access** to new content — all for the price of a coffee.

👉 **[Join CodingDroplets on Patreon](https://www.patreon.com/CodingDroplets)**

Prefer a one-time tip? [Buy us a coffee ☕](https://buymeacoffee.com/codingdroplets)

---

## 🎯 What You'll Learn

- How to build **custom ASP.NET Core middleware** from scratch
- How to **reuse an incoming `X-Correlation-ID`** or generate a new `Guid` when one isn't provided
- How to **propagate the correlation ID** back in the response headers for client-side tracing
- How to expose the correlation ID in **API response payloads** for easy log correlation
- How to **unit test middleware** behaviour with `HttpContext` mocking

---

## 🗺️ Architecture Overview

```
Client Request
  (with or without X-Correlation-ID header)
        │
        ▼
┌──────────────────────────────────────────────────────┐
│         CorrelationIdMiddleware  ← EARLY             │
│                                                      │
│  Has X-Correlation-ID header?                        │
│  YES → reuse the incoming ID                         │
│  NO  → generate new Guid.NewGuid()                   │
│                                                      │
│  → Store in HttpContext.Items["CorrelationId"]        │
│  → Add to response: X-Correlation-ID header          │
└──────────────────────────────────────────────────────┘
        │
        ▼
┌──────────────────────────────────────────────────────┐
│              API Controller / Endpoint               │
│  Reads CorrelationId from HttpContext.Items           │
│  Includes it in the response payload                 │
│  Logs it with Serilog / ILogger for traceability     │
└──────────────────────────────────────────────────────┘
        │
        ▼
HTTP Response
  Headers: X-Correlation-ID: <guid>
  Body:    { "correlationId": "<guid>", "data": ... }
```

---

## 📋 Middleware Behaviour

| Scenario | Behaviour |
|----------|-----------|
| Request **with** `X-Correlation-ID` header | Middleware reuses the provided ID |
| Request **without** `X-Correlation-ID` header | Middleware generates a new `Guid` |
| All responses | `X-Correlation-ID` header is always returned |
| API endpoints | Correlation ID available via `HttpContext.Items` |

---

## 📁 Project Structure

```
dotnet-request-correlation-middleware/
├── CodingDroplets.RequestCorrelationMiddleware.sln
├── CodingDroplets.RequestCorrelationMiddleware.Api/
│   ├── Middleware/
│   │   └── CorrelationIdMiddleware.cs    # Custom middleware implementation
│   ├── Controllers/
│   │   └── SampleController.cs          # Demo endpoint returning correlation ID
│   ├── Program.cs                       # Middleware registration
│   └── CodingDroplets.RequestCorrelationMiddleware.Api.csproj
├── CodingDroplets.RequestCorrelationMiddleware.Tests/
│   ├── CorrelationIdMiddlewareTests.cs  # Unit tests for middleware behaviour
│   └── CodingDroplets.RequestCorrelationMiddleware.Tests.csproj
├── CHANGELOG.md
└── LICENSE
```

---

## 🛠️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Any IDE: Visual Studio 2022+, VS Code, or JetBrains Rider

---

## ⚡ Quick Start

```bash
# Clone the repo
git clone https://github.com/codingdroplets/dotnet-request-correlation-middleware.git
cd dotnet-request-correlation-middleware

# Run the API
dotnet run --project CodingDroplets.RequestCorrelationMiddleware.Api

# Open Swagger UI → http://localhost:{port}/swagger
```

---

## 🔧 How It Works

### Step 1 — Implement the Middleware

```csharp
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Reuse incoming ID or generate a new one
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var incoming)
            ? incoming.ToString()
            : Guid.NewGuid().ToString();

        // Make it available downstream
        context.Items["CorrelationId"] = correlationId;

        // Always return it in the response
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
```

### Step 2 — Register in Program.cs

```csharp
// Register early — before routing and endpoints
app.UseMiddleware<CorrelationIdMiddleware>();
```

### Step 3 — Use in Your Endpoints / Controllers

```csharp
[HttpGet("status")]
public IActionResult GetStatus()
{
    var correlationId = HttpContext.Items["CorrelationId"]?.ToString();

    return Ok(new
    {
        status = "healthy",
        correlationId,
        timestamp = DateTime.UtcNow
    });
}
```

---

## 🧪 Running Tests

```bash
dotnet test CodingDroplets.RequestCorrelationMiddleware.sln
```

Unit tests verify:
- Middleware **generates** a new correlation ID when none is provided
- Middleware **reuses** the incoming `X-Correlation-ID` header value
- Response always contains the `X-Correlation-ID` header
- `HttpContext.Items["CorrelationId"]` is populated correctly

---

## 🤔 Key Concepts

### Why Correlation IDs Matter in Distributed Systems

| Without Correlation IDs | With Correlation IDs |
|------------------------|----------------------|
| "Which log entry matches the client error?" → impossible | Every log line tagged with the same ID |
| Debugging cross-service failures takes hours | Grep by ID across all service logs |
| Client has no way to report a specific request | Client can include the ID in a support ticket |
| No audit trail for individual requests | Full request lifecycle traceable |

### Reuse vs Generate

Always **reuse** the incoming ID if provided — this allows a gateway, load balancer, or upstream service to stamp the ID before it reaches your API, maintaining a consistent ID across the entire call chain.

---

## 📚 References

- [Write custom ASP.NET Core middleware — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write)
- [Logging and tracing in distributed systems — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/)
- [W3C Trace Context specification](https://www.w3.org/TR/trace-context/)

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🔗 Connect with CodingDroplets

| Platform | Link |
|----------|------|
| 🌐 Website | https://codingdroplets.com/ |
| 📺 YouTube | https://www.youtube.com/@CodingDroplets |
| 🎁 Patreon | https://www.patreon.com/CodingDroplets |
| ☕ Buy Me a Coffee | https://buymeacoffee.com/codingdroplets |
| 💻 GitHub | http://github.com/codingdroplets/ |

> **Want more samples like this?** [Support us on Patreon](https://www.patreon.com/CodingDroplets) or [buy us a coffee ☕](https://buymeacoffee.com/codingdroplets) — every bit helps keep the content coming!
