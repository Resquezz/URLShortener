using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.DTOs;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Application.Services
{
    public class URLShortenerService(IShortenedURLRepository _shortenedURLRepository,
        IHttpContextAccessor _httpContextAccessor) : IURLShortenerService
    {
        private const string AllowedSymbols = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int ShortCodeLength = 6;
        private const int MaxCodeGenerationAttempts = 20;
        public async Task DeleteRecordAsync(Guid id)
        {
            var url = await _shortenedURLRepository.GetShortenedURLByIdAsync(id);
            if (url == null)
            {
                throw new KeyNotFoundException($"URL with id {id} not found.");
            }

            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid userId;
            var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                userId = Guid.Parse(userIdClaim);
            }
            else
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            if(url.CreatedById != userId && userRole != "Admin")
            {
                throw new UnauthorizedAccessException("User is not authorized to delete this URL.");
            }

            await _shortenedURLRepository.DeleteShortenedURLAsync(url);
        }

        public async Task<string> GetLongUrlAsync(string shortCode)
        {
            var url = await _shortenedURLRepository.GetShortenedURLByCodeAsync(shortCode);
            if (url == null)
            {
                throw new KeyNotFoundException($"URL with code {shortCode} not found.");
            }

            return url.LongURL;
        }

        public async Task<ICollection<ShortenedURL>> GetShortenedURLsAsync()
        {
            var result = await _shortenedURLRepository.GetAllShortenedURLsAsync();
            return result;
        }

        public async Task<ShortUrlInfoResponse> GetShortenedURLInfoAsync(Guid id)
        {
            var url = await _shortenedURLRepository.GetShortenedURLByIdAsync(id);
            if (url == null)
            {
                throw new KeyNotFoundException($"URL with id {id} not found.");
            }

            return new ShortUrlInfoResponse
            {
                Id = url.Id,
                LongUrl = url.LongURL,
                ShortCode = url.ShortCode,
                CreatedAt = url.CreatedAt,
                CreatedByUsername = url.CreatedBy.Username
            };
        }

        public async Task<string> ShortenUrlAsync(string longUrl)
        {
            if (string.IsNullOrWhiteSpace(longUrl))
            {
                throw new ArgumentException("Long URL cannot be empty.", nameof(longUrl));
            }

            var normalizedLongUrl = longUrl.Trim();
            var userId = GetCurrentUserId();

            var existingUrl = await _shortenedURLRepository.GetShortenedURLByLongUrlAsync(normalizedLongUrl);
            if (existingUrl != null)
            {
                throw new InvalidOperationException($"URL {normalizedLongUrl} has already been shortened.");
            }

            for (var attempt = 0; attempt < MaxCodeGenerationAttempts; attempt++)
            {
                var code = GenerateCode(ShortCodeLength);
                var codeExists = await _shortenedURLRepository.ShortCodeExistsAsync(code);
                if (!codeExists)
                {
                    await _shortenedURLRepository.CreateShortenedURLAsync(normalizedLongUrl, code, userId);
                    return code;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique short URL code.");
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            return Guid.Parse(userIdClaim);
        }

        private string GenerateCode(int length)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(AllowedSymbols, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
