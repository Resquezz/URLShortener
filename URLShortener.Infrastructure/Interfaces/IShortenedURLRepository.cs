using URLShortener.Domain.Models;

namespace URLShortener.Infrastructure.Interfaces
{
    public interface IShortenedURLRepository
    {
        Task CreateShortenedURLAsync(string longUrl, string shortCode, Guid createdById);
        Task<ShortenedURL?> GetShortenedURLByCodeAsync(string shortCode);
        Task<ShortenedURL?> GetShortenedURLByLongUrlAsync(string longURL);
        Task<ShortenedURL?> GetShortenedURLByIdAsync(Guid id);
        Task<bool> ShortCodeExistsAsync(string shortCode);
        Task DeleteShortenedURLAsync(ShortenedURL url);
        Task<ICollection<ShortenedURL>> GetAllShortenedURLsAsync();
    }
}
