using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.DTOs;

namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLShortenerController : Controller
    {
        private readonly IURLShortenerService _urlShortenerService;
        public URLShortenerController(IURLShortenerService uRLShortenerService)
        {
            _urlShortenerService = uRLShortenerService;
        }

        [HttpGet("/r/{code}")]
        public async Task<IActionResult> ShortenUrl(string code)
        {
            var longURL = await _urlShortenerService.GetLongUrlAsync(code);
            return Redirect(longURL);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNewRecord([FromBody] CreateShortUrlRequest request)
        {
            var shortURL = await _urlShortenerService.ShortenUrlAsync(request.LongUrl);
            return Ok(shortURL);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteRecord(Guid id)
        {
            await _urlShortenerService.DeleteRecordAsync(id);
            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRecords()
        {
            var records = await _urlShortenerService.GetShortenedURLsAsync();
            return Ok(records);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetRecordInfo(Guid id)
        {
            var recordInfo = await _urlShortenerService.GetShortenedURLInfoAsync(id);
            return Ok(recordInfo);
        }
    }
}
