using Microsoft.AspNetCore.Mvc;
using Moq;
using URLShortener.Application.Interfaces;
using URLShortener.Controllers;
using URLShortener.Domain.DTOs;

namespace URLShortener.Tests.Controllers;

public class UsersControllerTests
{
    [Fact]
    public async Task Register_ValidRequest_ReturnsOkWithMessage()
    {
        var userService = new Mock<IUserService>();
        var controller = new UsersController(userService.Object);
        var request = new AuthenticationRequest { Username = "user1", Password = "password" };

        var result = await controller.Register(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        userService.Verify(service => service.RegisterAsync(request), Times.Once);
    }

    [Fact]
    public async Task Register_ServiceThrows_BubblesException()
    {
        var userService = new Mock<IUserService>();
        var controller = new UsersController(userService.Object);
        var request = new AuthenticationRequest { Username = "user1", Password = "password" };

        userService.Setup(service => service.RegisterAsync(request))
            .ThrowsAsync(new InvalidOperationException("Username already exists."));

        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Register(request));
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var userService = new Mock<IUserService>();
        var controller = new UsersController(userService.Object);
        var request = new AuthenticationRequest { Username = "user1", Password = "password" };

        userService.Setup(service => service.LoginAsync(request))
            .ReturnsAsync("jwt-token");

        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        userService.Verify(service => service.LoginAsync(request), Times.Once);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var userService = new Mock<IUserService>();
        var controller = new UsersController(userService.Object);
        var request = new AuthenticationRequest { Username = "user1", Password = "bad" };

        userService.Setup(service => service.LoginAsync(request))
            .ReturnsAsync((string?)null);

        var result = await controller.Login(request);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
        userService.Verify(service => service.LoginAsync(request), Times.Once);
    }

    [Fact]
    public async Task Login_ServiceThrows_BubblesException()
    {
        var userService = new Mock<IUserService>();
        var controller = new UsersController(userService.Object);
        var request = new AuthenticationRequest { Username = "user1", Password = "password" };

        userService.Setup(service => service.LoginAsync(request))
            .ThrowsAsync(new ArgumentNullException(nameof(request)));

        await Assert.ThrowsAsync<ArgumentNullException>(() => controller.Login(request));
    }
}
