using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using URLShortener.Domain.Enums;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Data;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Infrastructure.Repositories
{
    public class UserRepository(ShortenerDbContext _context) : IUserRepository
    {
        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
