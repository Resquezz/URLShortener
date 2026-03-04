using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using URLShortener.Domain.Enums;

namespace URLShortener.Domain.Models
{
    public class User
    {
        public User(string username, string passwordHash)
        {
            Username = username;
            PasswordHash = passwordHash;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public Role Role { get; set; } = Role.User;

        public ICollection<ShortenedURL> ShortenedURLs { get; set; } = null!;
    }
}
