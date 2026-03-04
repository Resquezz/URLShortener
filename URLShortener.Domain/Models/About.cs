using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace URLShortener.Domain.Models
{
    public class About
    {
        public About(string content)
        {
            Content = content;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Content { get; set; } = null!;
    }
}
