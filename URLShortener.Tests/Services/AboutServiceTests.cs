using Moq;
using URLShortener.Application.Services;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Tests.Services;

public class AboutServiceTests
{
    [Fact]
    public async Task CreateEmptyAboutAsync_WhenAboutExists_DoesNotCreateNew()
    {
        var repository = new Mock<IAboutRepository>();
        var service = new AboutService(repository.Object);

        repository.Setup(r => r.GetAboutAsync())
            .ReturnsAsync(new About("existing"));

        await service.CreateEmptyAboutAsync();

        repository.Verify(r => r.CreateEmptyAboutAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateEmptyAboutAsync_WhenAboutMissing_CreatesEmpty()
    {
        var repository = new Mock<IAboutRepository>();
        var service = new AboutService(repository.Object);

        repository.Setup(r => r.GetAboutAsync())
            .ReturnsAsync((About?)null);

        await service.CreateEmptyAboutAsync();

        repository.Verify(r => r.CreateEmptyAboutAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAboutContentAsync_WhenAboutMissing_CreatesAndReturnsContent()
    {
        var repository = new Mock<IAboutRepository>();
        var service = new AboutService(repository.Object);

        repository.SetupSequence(r => r.GetAboutAsync())
            .ReturnsAsync((About?)null)
            .ReturnsAsync(new About(string.Empty));
        repository.Setup(r => r.GetAboutContentAsync())
            .ReturnsAsync(string.Empty);

        var content = await service.GetAboutContentAsync();

        Assert.Equal(string.Empty, content);
        repository.Verify(r => r.CreateEmptyAboutAsync(), Times.Once);
        repository.Verify(r => r.GetAboutContentAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAboutContentAsync_WhenAboutMissing_CreatesThenUpdates()
    {
        var repository = new Mock<IAboutRepository>();
        var service = new AboutService(repository.Object);
        var about = new About(string.Empty);

        repository.SetupSequence(r => r.GetAboutAsync())
            .ReturnsAsync((About?)null)
            .ReturnsAsync(about);

        await service.UpdateAboutContentAsync("updated");

        Assert.Equal("updated", about.Content);
        repository.Verify(r => r.CreateEmptyAboutAsync(), Times.Once);
        repository.Verify(r => r.UpdateAboutAsync(about), Times.Once);
    }

    [Fact]
    public async Task UpdateAboutContentAsync_WhenContentNull_ThrowsArgumentNullException()
    {
        var repository = new Mock<IAboutRepository>();
        var service = new AboutService(repository.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateAboutContentAsync(null!));

        repository.Verify(r => r.CreateEmptyAboutAsync(), Times.Never);
        repository.Verify(r => r.UpdateAboutAsync(It.IsAny<About>()), Times.Never);
    }
}
