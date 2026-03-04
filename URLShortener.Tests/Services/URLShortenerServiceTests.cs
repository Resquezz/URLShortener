using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using URLShortener.Application.Services;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Tests.Services;

public class URLShortenerServiceTests
{
    [Fact]
    public async Task ShortenUrlAsync_WhenLongUrlEmpty_ThrowsArgumentException()
    {
        var repository = new Mock<IShortenedURLRepository>();
        var accessor = CreateHttpContextAccessor(Guid.NewGuid(), "User");
        var service = new URLShortenerService(repository.Object, accessor);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ShortenUrlAsync(" "));
    }

    [Fact]
    public async Task ShortenUrlAsync_WhenDuplicate_ThrowsInvalidOperationException()
    {
        var repository = new Mock<IShortenedURLRepository>();
        var accessor = CreateHttpContextAccessor(Guid.NewGuid(), "User");
        var service = new URLShortenerService(repository.Object, accessor);

        repository.Setup(r => r.GetShortenedURLByLongUrlAsync("https://example.com"))
            .ReturnsAsync(new ShortenedURL("https://example.com", "abc123", DateTime.UtcNow, Guid.NewGuid()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ShortenUrlAsync("https://example.com"));
    }

    [Fact]
    public async Task ShortenUrlAsync_WhenValid_CreatesRecordAndReturnsShortCode()
    {
        var userId = Guid.NewGuid();
        var repository = new Mock<IShortenedURLRepository>();
        var accessor = CreateHttpContextAccessor(userId, "User");
        var service = new URLShortenerService(repository.Object, accessor);

        repository.Setup(r => r.GetShortenedURLByLongUrlAsync("https://example.com"))
            .ReturnsAsync((ShortenedURL?)null);
        repository.Setup(r => r.ShortCodeExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var shortCode = await service.ShortenUrlAsync("https://example.com");

        Assert.Equal(6, shortCode.Length);
        repository.Verify(r => r.CreateShortenedURLAsync("https://example.com", shortCode, userId), Times.Once);
    }

    [Fact]
    public async Task DeleteRecordAsync_WhenNoUserClaim_ThrowsInvalidOperationException()
    {
        var url = new ShortenedURL("https://example.com", "abc123", DateTime.UtcNow, Guid.NewGuid());
        var repository = new Mock<IShortenedURLRepository>();
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, "User")
                }, "TestAuth"))
            }
        };

        repository.Setup(r => r.GetShortenedURLByIdAsync(url.Id)).ReturnsAsync(url);
        var service = new URLShortenerService(repository.Object, accessor);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRecordAsync(url.Id));
    }

    [Fact]
    public async Task DeleteRecordAsync_WhenNotOwnerAndNotAdmin_ThrowsUnauthorizedAccessException()
    {
        var createdById = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var url = new ShortenedURL("https://example.com", "abc123", DateTime.UtcNow, createdById);

        var repository = new Mock<IShortenedURLRepository>();
        repository.Setup(r => r.GetShortenedURLByIdAsync(url.Id)).ReturnsAsync(url);

        var accessor = CreateHttpContextAccessor(currentUserId, "User");
        var service = new URLShortenerService(repository.Object, accessor);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.DeleteRecordAsync(url.Id));
    }

    [Fact]
    public async Task DeleteRecordAsync_WhenOwner_DeletesRecord()
    {
        var ownerId = Guid.NewGuid();
        var url = new ShortenedURL("https://example.com", "abc123", DateTime.UtcNow, ownerId);

        var repository = new Mock<IShortenedURLRepository>();
        repository.Setup(r => r.GetShortenedURLByIdAsync(url.Id)).ReturnsAsync(url);

        var accessor = CreateHttpContextAccessor(ownerId, "User");
        var service = new URLShortenerService(repository.Object, accessor);

        await service.DeleteRecordAsync(url.Id);

        repository.Verify(r => r.DeleteShortenedURLAsync(url), Times.Once);
    }

    [Fact]
    public async Task DeleteRecordAsync_WhenAdmin_DeletesRecord()
    {
        var createdById = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var url = new ShortenedURL("https://example.com", "abc123", DateTime.UtcNow, createdById);

        var repository = new Mock<IShortenedURLRepository>();
        repository.Setup(r => r.GetShortenedURLByIdAsync(url.Id)).ReturnsAsync(url);

        var accessor = CreateHttpContextAccessor(adminId, "Admin");
        var service = new URLShortenerService(repository.Object, accessor);

        await service.DeleteRecordAsync(url.Id);

        repository.Verify(r => r.DeleteShortenedURLAsync(url), Times.Once);
    }

    private static HttpContextAccessor CreateHttpContextAccessor(Guid userId, string role)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        }, "TestAuth");

        return new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }
}
