using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Data;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Infrastructure.Repositories
{
    public class ShortenedURLRepository(ShortenerDbContext _context) : IShortenedURLRepository
    {
        public async Task CreateShortenedURLAsync(string longUrl, string shortCode, Guid createdById)
        {
            var shortenedURL = new ShortenedURL(longUrl, shortCode, DateTime.Now, createdById);
            _context.ShortenedURLs.Add(shortenedURL);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShortenedURLAsync(ShortenedURL url)
        {
            _context.ShortenedURLs.Remove(url);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<ShortenedURL>> GetAllShortenedURLsAsync()
        {
            return await _context.ShortenedURLs.ToListAsync();
        }

        public async Task<ShortenedURL?> GetShortenedURLByCodeAsync(string shortCode)
        {
            return await _context.ShortenedURLs.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
        }

        public async Task<ShortenedURL?> GetShortenedURLByIdAsync(Guid id)
        {
            return await _context.ShortenedURLs
                .Include(u => u.CreatedBy)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ShortenedURL?> GetShortenedURLByLongUrlAsync(string longURL)
        {
            return await _context.ShortenedURLs.FirstOrDefaultAsync(u => u.LongURL == longURL);
        }

        public async Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            return await _context.ShortenedURLs.AnyAsync(u => u.ShortCode == shortCode);
        }
    }
}
