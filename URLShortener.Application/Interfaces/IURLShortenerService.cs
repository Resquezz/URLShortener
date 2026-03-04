using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.DTOs;
using URLShortener.Domain.Models;

namespace URLShortener.Application.Interfaces
{
    public interface IURLShortenerService
    {
        Task<string> GetLongUrlAsync(string code);
        Task<string> ShortenUrlAsync(string longUrl);
        Task DeleteRecordAsync(Guid id);
        Task<ICollection<ShortenedURL>> GetShortenedURLsAsync();
        Task<ShortUrlInfoResponse> GetShortenedURLInfoAsync(Guid id);
    }
}
