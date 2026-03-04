using URLShortener.Domain.Models;

namespace URLShortener.Infrastructure.Interfaces
{
    public interface IAboutRepository
    {
        Task<string> GetAboutContentAsync();
        Task<About?> GetAboutAsync();
        Task UpdateAboutAsync(About about);
        Task CreateEmptyAboutAsync();
    }
}
