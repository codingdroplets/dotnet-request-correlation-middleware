var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Add correlation-id middleware early so every request/response gets traced consistently.
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapGet("/api/orders/{id:int}", (int id, HttpContext context) =>
{
    // Read correlation id from HttpContext items, populated by middleware.
    var correlationId = context.Items[CorrelationIdMiddleware.CorrelationItemKey]?.ToString() ?? "unknown";

    return Results.Ok(new OrderResponse(
        OrderId: id,
        Status: "Processing",
        Message: "Order status fetched successfully.",
        CorrelationId: correlationId));
})
.WithName("GetOrderStatus")
.WithSummary("Get order status with request correlation ID")
.WithOpenApi();

app.Run();

public partial class Program { }

public record OrderResponse(int OrderId, string Status, string Message, string CorrelationId);

public class CorrelationIdMiddleware
{
    public const string CorrelationHeaderName = "X-Correlation-ID";
    public const string CorrelationItemKey = "CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Reuse incoming correlation ID when provided; otherwise generate a new one.
        var correlationId = context.Request.Headers[CorrelationHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        // Store for downstream handlers and mirror it in response headers.
        context.Items[CorrelationItemKey] = correlationId;
        context.Response.Headers[CorrelationHeaderName] = correlationId;

        await _next(context);
    }
}
