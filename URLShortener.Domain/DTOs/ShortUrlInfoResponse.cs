namespace URLShortener.Domain.DTOs
{
    public class ShortUrlInfoResponse
    {
        public Guid Id { get; set; }
        public string LongUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string CreatedByUsername { get; set; } = null!;
    }
}