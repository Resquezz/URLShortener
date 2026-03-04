using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Application.Services
{
    public class AboutService(IAboutRepository _aboutRepository) : IAboutService
    {
        public async Task CreateEmptyAboutAsync()
        {
            var existingAbout = await _aboutRepository.GetAboutAsync();
            if (existingAbout == null)
            {
                await _aboutRepository.CreateEmptyAboutAsync();
            }
        }

        public async Task<string?> GetAboutContentAsync()
        {
            await CreateEmptyAboutAsync();
            return await _aboutRepository.GetAboutContentAsync();
        }

        public async Task UpdateAboutContentAsync(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            await CreateEmptyAboutAsync();

            var about = await _aboutRepository.GetAboutAsync();
            if (about == null)
            {
                throw new InvalidOperationException("Failed to initialize About content.");
            }

            about.Content = content;
            await _aboutRepository.UpdateAboutAsync(about);
        }
    }
}
