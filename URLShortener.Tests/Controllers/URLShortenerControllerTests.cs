using Microsoft.AspNetCore.Mvc;
using Moq;
using URLShortener.Application.Interfaces;
using URLShortener.Controllers;
using URLShortener.Domain.DTOs;
using URLShortener.Domain.Models;

namespace URLShortener.Tests.Controllers;

public class URLShortenerControllerTests
{
    [Fact]
    public async Task ShortenUrl_WhenCodeExists_RedirectsToLongUrl()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);

        service.Setup(s => s.GetLongUrlAsync("abc123"))
            .ReturnsAsync("https://example.com");

        var result = await controller.ShortenUrl("abc123");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("https://example.com", redirect.Url);
    }

    [Fact]
    public async Task ShortenUrl_WhenCodeNotFound_BubblesException()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);

        service.Setup(s => s.GetLongUrlAsync("missing"))
            .ThrowsAsync(new KeyNotFoundException("URL not found."));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.ShortenUrl("missing"));
    }

    [Fact]
    public async Task CreateNewRecord_ValidLongUrl_ReturnsOkWithShortCode()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);

        service.Setup(s => s.ShortenUrlAsync("https://example.com"))
            .ReturnsAsync("rNZ3D4");

        var result = await controller.CreateNewRecord(new CreateShortUrlRequest { LongUrl = "https://example.com" });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("rNZ3D4", ok.Value);
    }

    [Fact]
    public async Task CreateNewRecord_DuplicateUrl_BubblesException()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);

        service.Setup(s => s.ShortenUrlAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("already shortened"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.CreateNewRecord(new CreateShortUrlRequest { LongUrl = "https://example.com" }));
    }

    [Fact]
    public async Task DeleteRecord_WhenAllowed_ReturnsNoContent()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);
        var id = Guid.NewGuid();

        var result = await controller.DeleteRecord(id);

        Assert.IsType<NoContentResult>(result);
        service.Verify(s => s.DeleteRecordAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteRecord_WhenForbidden_BubblesException()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);
        var id = Guid.NewGuid();

        service.Setup(s => s.DeleteRecordAsync(id))
            .ThrowsAsync(new UnauthorizedAccessException("Forbidden"));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.DeleteRecord(id));
    }

    [Fact]
    public async Task DeleteRecord_WhenNotFound_BubblesException()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);
        var id = Guid.NewGuid();

        service.Setup(s => s.DeleteRecordAsync(id))
            .ThrowsAsync(new KeyNotFoundException("not found"));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.DeleteRecord(id));
    }

    [Fact]
    public async Task GetAllRecords_WhenAnyExist_ReturnsOkWithCollection()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);

        service.Setup(s => s.GetShortenedURLsAsync())
            .ReturnsAsync(new List<ShortenedURL>
            {
                new("https://a.com", "aaaaaa", DateTime.UtcNow, Guid.NewGuid()),
                new("https://b.com", "bbbbbb", DateTime.UtcNow, Guid.NewGuid())
            });

        var result = await controller.GetAllRecords();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task GetAllRecords_WhenEmpty_ReturnsOkWithEmptyCollection()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);

        service.Setup(s => s.GetShortenedURLsAsync())
            .ReturnsAsync(new List<ShortenedURL>());

        var result = await controller.GetAllRecords();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task GetRecordInfo_WhenExists_ReturnsOkWithRecord()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);
        var id = Guid.NewGuid();

        service.Setup(s => s.GetShortenedURLInfoAsync(id))
            .ReturnsAsync(new ShortUrlInfoResponse
            {
                Id = id,
                LongUrl = "https://example.com",
                ShortCode = "abc123",
                CreatedAt = DateTime.UtcNow,
                CreatedByUsername = "user1"
            });

        var result = await controller.GetRecordInfo(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ShortUrlInfoResponse>(ok.Value);
        Assert.Equal(id, payload.Id);
    }

    [Fact]
    public async Task GetRecordInfo_WhenNotFound_BubblesException()
    {
        var service = new Mock<IURLShortenerService>();
        var controller = new URLShortenerController(service.Object);
        var id = Guid.NewGuid();

        service.Setup(s => s.GetShortenedURLInfoAsync(id))
            .ThrowsAsync(new KeyNotFoundException("URL with id not found"));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.GetRecordInfo(id));
    }
}
