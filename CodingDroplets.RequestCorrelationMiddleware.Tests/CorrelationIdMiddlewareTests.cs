using Microsoft.AspNetCore.Http;

namespace CodingDroplets.RequestCorrelationMiddleware.Tests;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task Invoke_Should_Generate_CorrelationId_When_Header_Missing()
    {
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.Invoke(context);

        Assert.True(context.Response.Headers.ContainsKey(CorrelationIdMiddleware.CorrelationHeaderName));
        var correlationId = context.Response.Headers[CorrelationIdMiddleware.CorrelationHeaderName].ToString();
        Assert.False(string.IsNullOrWhiteSpace(correlationId));
    }

    [Fact]
    public async Task Invoke_Should_Reuse_Incoming_CorrelationId_When_Header_Present()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.CorrelationHeaderName] = "abc-123";

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);
        await middleware.Invoke(context);

        Assert.Equal("abc-123", context.Response.Headers[CorrelationIdMiddleware.CorrelationHeaderName].ToString());
        Assert.Equal("abc-123", context.Items[CorrelationIdMiddleware.CorrelationItemKey]?.ToString());
    }
}
