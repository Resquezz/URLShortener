using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using URLShortener.Middlewares;

namespace URLShortener.Tests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Invoke_NoException_CallsNextAndKeepsDefaultStatus()
    {
        var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(_ => Task.CompletedTask, logger.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Theory]
    [InlineData("arg-null", StatusCodes.Status400BadRequest)]
    [InlineData("arg", StatusCodes.Status400BadRequest)]
    [InlineData("not-found", StatusCodes.Status404NotFound)]
    [InlineData("forbidden", StatusCodes.Status403Forbidden)]
    [InlineData("conflict", StatusCodes.Status409Conflict)]
    [InlineData("unknown", StatusCodes.Status500InternalServerError)]
    public async Task Invoke_ExceptionThrown_ReturnsMappedStatusAndJsonBody(string kind, int expectedStatus)
    {
        static Exception CreateException(string key)
        {
            return key switch
            {
                "arg-null" => new ArgumentNullException("x", "arg null"),
                "arg" => new ArgumentException("arg"),
                "not-found" => new KeyNotFoundException("missing"),
                "forbidden" => new UnauthorizedAccessException("forbidden"),
                "conflict" => new InvalidOperationException("conflict"),
                _ => new Exception("unknown")
            };
        }

        var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(_ => throw CreateException(kind), logger.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal(expectedStatus, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();
        using var document = JsonDocument.Parse(body);
        Assert.True(document.RootElement.TryGetProperty("title", out _));
        Assert.True(document.RootElement.TryGetProperty("status", out _));
        if (expectedStatus == StatusCodes.Status500InternalServerError)
        {
            Assert.False(document.RootElement.TryGetProperty("detail", out var detail500) && detail500.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(detail500.GetString()));
        }
        else
        {
            Assert.True(document.RootElement.TryGetProperty("detail", out var detail));
            Assert.Equal(JsonValueKind.String, detail.ValueKind);
            Assert.False(string.IsNullOrWhiteSpace(detail.GetString()));
        }
    }
}
