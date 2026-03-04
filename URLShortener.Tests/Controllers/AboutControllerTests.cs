using Microsoft.AspNetCore.Mvc;
using Moq;
using URLShortener.Application.Interfaces;
using URLShortener.Controllers;
using URLShortener.Domain.DTOs;

namespace URLShortener.Tests.Controllers;

public class AboutControllerTests
{
    [Fact]
    public async Task CreateEmptyAbout_ServiceSucceeds_ReturnsOk()
    {
        var aboutService = new Mock<IAboutService>();
        var controller = new AboutController(aboutService.Object);

        var result = await controller.CreateEmptyAbout();

        Assert.IsType<OkResult>(result);
        aboutService.Verify(service => service.CreateEmptyAboutAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateEmptyAbout_ServiceThrows_BubblesException()
    {
        var aboutService = new Mock<IAboutService>();
        var controller = new AboutController(aboutService.Object);

        aboutService.Setup(service => service.CreateEmptyAboutAsync())
            .ThrowsAsync(new InvalidOperationException("Already exists"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.CreateEmptyAbout());
    }

    [Fact]
    public async Task GetAboutContent_WhenExists_ReturnsOkWithContent()
    {
        var aboutService = new Mock<IAboutService>();
        var controller = new AboutController(aboutService.Object);

        aboutService.Setup(service => service.GetAboutContentAsync())
            .ReturnsAsync("Algorithm description");

        var result = await controller.GetAboutContent();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        aboutService.Verify(service => service.GetAboutContentAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAboutContent_WhenNull_ReturnsOkWithNullContent()
    {
        var aboutService = new Mock<IAboutService>();
        var controller = new AboutController(aboutService.Object);

        aboutService.Setup(service => service.GetAboutContentAsync())
            .ReturnsAsync((string?)null);

        var result = await controller.GetAboutContent();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task UpdateAboutContent_ValidInput_ReturnsOk()
    {
        var aboutService = new Mock<IAboutService>();
        var controller = new AboutController(aboutService.Object);

        var result = await controller.UpdateAboutContent(new AboutContentUpdateRequest { Content = "Updated content" });

        Assert.IsType<OkResult>(result);
        aboutService.Verify(service => service.UpdateAboutContentAsync("Updated content"), Times.Once);
    }

    [Fact]
    public async Task UpdateAboutContent_ServiceThrows_BubblesException()
    {
        var aboutService = new Mock<IAboutService>();
        var controller = new AboutController(aboutService.Object);

        aboutService.Setup(service => service.UpdateAboutContentAsync(It.IsAny<string>()))
            .ThrowsAsync(new ArgumentNullException("content"));

        await Assert.ThrowsAsync<ArgumentNullException>(() => controller.UpdateAboutContent(new AboutContentUpdateRequest { Content = "" }));
    }
}
