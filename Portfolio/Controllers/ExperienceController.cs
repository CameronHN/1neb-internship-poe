using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.DTOs.Resume;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController(IExperienceService experienceService) : ControllerBase
    {
        private readonly IExperienceService _experienceService = experienceService;

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExperienceItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExperienceById(Guid id)
        {
            var userId = User.GetUserId()!.Value;

            var experience = await _experienceService.GetExperienceById(id, userId);
            return Ok(experience);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddExperiences([FromBody] List<AddExperience> experiences)
        {
            var userId = User.GetUserId()!.Value;

            if (experiences is null || experiences.Count == 0)
                return BadRequest("At least one experience is required.");

            var experienceIds = await _experienceService.AddExperiencesAsync(userId, experiences);
            return Created(string.Empty, experienceIds);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchExperience([FromBody] PatchExperience patch)
        {
            var userId = User.GetUserId()!.Value;

            if (patch is null)
                return BadRequest();

            var updated = await _experienceService.PatchExperienceAsync(userId, patch);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteExperiences([FromBody] List<Guid> experienceIds)
        {
            var userId = User.GetUserId()!.Value;

            if (experienceIds == null || experienceIds.Count == 0)
                return BadRequest("Experience IDs cannot be null or empty.");

            var deleted = await _experienceService.DeleteExperiencesAsync(userId, experienceIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified experiences.");

            return Ok(true);
        }
    }
}
