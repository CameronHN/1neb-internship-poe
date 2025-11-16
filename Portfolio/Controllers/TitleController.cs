using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.ResumeTitle;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
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
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddTitles([FromBody] List<AddResumeTitle> titles)
        {
            var userId = User.GetUserId()!.Value;

            if (titles is null)
                throw new ValidationException("Request body cannot be null.");

            var createdIds = await _titleService.AddTitlesAsync(userId, titles);

            return Created(string.Empty, createdIds);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetResumeTitleById([FromRoute] Guid id)
        {
            var title = await _titleService.GetResumeTitleById(id);
            if (title is null)
                return BadRequest();

            return Ok(title);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchTitle([FromBody] PatchResumeTitle patch)
        {
            var userId = User.GetUserId()!.Value;

            if (patch is null)
                throw new ValidationException("Request body cannot be null.");

            var updated = await _titleService.PatchTitleAsync(userId, patch);
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
            var userId = User.GetUserId()!.Value;

            if (titleIds == null || titleIds.Count == 0)
                return BadRequest("Title IDs cannot be null or empty.");

            var deleted = await _titleService.DeleteTitlesAsync(userId, titleIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified titles.");

            return Ok(true);
        }

        [HttpGet("titles")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTitles()
        {
            var userId = User.GetUserId()!.Value;

            var titles = await _titleService.GetTitlesByUserIdAsync(userId);
            return Ok(titles);
        }
    }
}
