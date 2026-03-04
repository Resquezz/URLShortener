using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Moq;
using URLShortener.Application.Services;
using URLShortener.Domain.DTOs;
using URLShortener.Domain.Enums;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenRequestNull_ThrowsArgumentNullException()
    {
        var repository = new Mock<IUserRepository>();
        var service = new UserService(repository.Object, CreateConfiguration());

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.RegisterAsync(null!));
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ThrowsInvalidOperationException()
    {
        var repository = new Mock<IUserRepository>();
        var service = new UserService(repository.Object, CreateConfiguration());
        var request = new AuthenticationRequest { Username = "user1", Password = "password123" };

        repository.Setup(r => r.GetUserByUsernameAsync("user1"))
            .ReturnsAsync(new User("user1", "hash"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_CreatesUserWithHashedPassword()
    {
        var repository = new Mock<IUserRepository>();
        var service = new UserService(repository.Object, CreateConfiguration());
        var request = new AuthenticationRequest { Username = "user1", Password = "password123" };

        repository.Setup(r => r.GetUserByUsernameAsync("user1"))
            .ReturnsAsync((User?)null);

        await service.RegisterAsync(request);

        repository.Verify(r => r.CreateUserAsync(It.Is<User>(u =>
            u.Username == "user1" &&
            u.PasswordHash != request.Password &&
            BCrypt.Net.BCrypt.Verify(request.Password, u.PasswordHash))), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserMissing_ReturnsNull()
    {
        var repository = new Mock<IUserRepository>();
        var service = new UserService(repository.Object, CreateConfiguration());

        repository.Setup(r => r.GetUserByUsernameAsync("user1"))
            .ReturnsAsync((User?)null);

        var token = await service.LoginAsync(new AuthenticationRequest { Username = "user1", Password = "password123" });

        Assert.Null(token);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ReturnsNull()
    {
        var repository = new Mock<IUserRepository>();
        var service = new UserService(repository.Object, CreateConfiguration());
        var user = new User("user1", BCrypt.Net.BCrypt.HashPassword("other-password"));

        repository.Setup(r => r.GetUserByUsernameAsync("user1"))
            .ReturnsAsync(user);

        var token = await service.LoginAsync(new AuthenticationRequest { Username = "user1", Password = "password123" });

        Assert.Null(token);
    }

    [Fact]
    public async Task LoginAsync_WhenValid_ReturnsJwtWithExpectedClaims()
    {
        var repository = new Mock<IUserRepository>();
        var service = new UserService(repository.Object, CreateConfiguration());
        var user = new User("user1", BCrypt.Net.BCrypt.HashPassword("password123")) { Role = Role.Admin };

        repository.Setup(r => r.GetUserByUsernameAsync("user1"))
            .ReturnsAsync(user);

        var token = await service.LoginAsync(new AuthenticationRequest { Username = "user1", Password = "password123" });

        Assert.False(string.IsNullOrWhiteSpace(token));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Name && c.Value == "user1");
        Assert.Contains(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");
    }

    private static IConfiguration CreateConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "A very strong and long JWT key for tests only 1234567890",
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Audience"] = "test-audience",
            ["Jwt:ExpireMinutes"] = "60"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}
