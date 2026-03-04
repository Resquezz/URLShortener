using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Application.Interfaces;
using URLShortener.Domain.DTOs;

namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutController(IAboutService _aboutService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmptyAbout()
        {
            await _aboutService.CreateEmptyAboutAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAboutContent()
        {
            var content = await _aboutService.GetAboutContentAsync();
            return Ok(new { content });
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAboutContent([FromBody] AboutContentUpdateRequest request)
        {
            await _aboutService.UpdateAboutContentAsync(request.Content);
            return Ok();
        }
    }
}
