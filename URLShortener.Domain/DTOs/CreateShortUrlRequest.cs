using System.ComponentModel.DataAnnotations;

namespace URLShortener.Domain.DTOs
{
    public class CreateShortUrlRequest
    {
        [Required]
        [Url]
        [StringLength(2048)]
        public string LongUrl { get; set; } = null!;
    }
}
