using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ResumeTitle;
using Portfolio.Core.Exceptions;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly ITitleService _titleService;

        public TitleController(ITitleService titleService)
        {
            _titleService = titleService;
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddTitle([FromBody] AddResumeTitle title)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (title is null)
                throw new ValidationException("Request body cannot be null.");

            title.UserId = userId.Value;

            var createdId = await _titleService.AddTitleAsync(title);

            return CreatedAtAction(nameof(GetTitleById), new { id = createdId }, createdId);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTitleById([FromRoute] Guid id)
        {
            var title = await _titleService.GetTitleById(id);
            if (title is null)
                return NotFound();

            return Ok(title);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchTitles([FromBody] List<PatchResumeTitle> patches)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _titleService.PatchTitlesAsync(userId.Value, patches);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteTitles([FromBody] List<Guid> titleIds)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (titleIds == null || !titleIds.Any())
                return BadRequest("Title IDs cannot be null or empty.");

            var deleted = await _titleService.DeleteTitlesAsync(userId.Value, titleIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified titles.");

            return Ok(true);
        }

        [HttpGet("titles")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTitles()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var titles = await _titleService.GetTitlesByUserIdAsync(userId.Value);
            return Ok(titles);
        }
    }
}
