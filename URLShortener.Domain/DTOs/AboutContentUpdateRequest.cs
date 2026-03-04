using System.ComponentModel.DataAnnotations;

namespace URLShortener.Domain.DTOs
{
    public class AboutContentUpdateRequest
    {
        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = null!;
    }
}
