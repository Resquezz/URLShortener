using URLShortener.Domain.Models;

namespace URLShortener.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(Guid id);
    }
}
