using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace URLShortener.Domain.Models
{
    public class ShortenedURL
    {
        public ShortenedURL(string longURL, string shortCode, DateTime createdAt, Guid createdById)
        {
            LongURL = longURL;
            ShortCode = shortCode;
            CreatedAt = createdAt;
            CreatedById = createdById;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string LongURL { get; set; } = null!;

        [Required]
        public string ShortCode { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Guid CreatedById { get; set; }

        public User CreatedBy { get; set; } = null!;
    }
}
