using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.DTOs;
using URLShortener.Domain.Enums;
using URLShortener.Domain.Models;
using URLShortener.Infrastructure.Interfaces;

namespace URLShortener.Application.Services
{
    public class UserService(IUserRepository _userRepository, IConfiguration _config) : IUserService
    {
        public async Task<string?> LoginAsync(AuthenticationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Register user request can not be null.");
            }

            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (user == null)
            {
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]
                ?? throw new KeyNotFoundException("Jwt key not found.")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var parsedExpireMinutes)
                ? parsedExpireMinutes
                : 60;

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RegisterAsync(AuthenticationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Register user request can not be null.");
            }

            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User(request.Username, passwordHash);

            await _userRepository.CreateUserAsync(user);
        }
    }
}
