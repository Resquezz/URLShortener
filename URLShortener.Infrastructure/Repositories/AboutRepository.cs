using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Data;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Infrastructure.Repositories
{
    public class AboutRepository(ShortenerDbContext _context) : IAboutRepository
    {
        public async Task CreateEmptyAboutAsync()
        {
            var about = new About(string.Empty);

            _context.Abouts.Add(about);
            await _context.SaveChangesAsync();
        }

        public async Task<About?> GetAboutAsync()
        {
            return await _context.Abouts.FirstOrDefaultAsync();
        }

        public async Task<string> GetAboutContentAsync()
        {
            return (await _context.Abouts.FirstOrDefaultAsync())?.Content ?? string.Empty;
        }

        public async Task UpdateAboutAsync(About about)
        {
            _context.Abouts.Update(about);
            await _context.SaveChangesAsync();
        }
    }
}
