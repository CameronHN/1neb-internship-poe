using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
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
            var experience = await _experienceService.GetExperienceById(id);
            return Ok(experience);
        }

        [HttpGet("experiences")]
        [ProducesResponseType(typeof(List<ExperienceItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetExperienceItemsByUserId()
        {
            var userId = User.GetUserId()!.Value;

            var experiences =
                await _experienceService.GetAllExperiencesIncludingResponsibilitiesByUserIdAsync(
                    userId
                );
            return Ok(experiences);
        }

        [HttpPost("experiences")]
        [ProducesResponseType(typeof(List<ExperienceItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllExperiencesByIds([FromBody] ItemListRequest request)
        {
            var experiences = await _experienceService.GetAllExperiencesByIds(request);
            return Ok(experiences);
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
        public async Task<IActionResult> PatchExperiences([FromBody] List<PatchExperience> patches)
        {
            var userId = User.GetUserId()!.Value;

            if (patches is null)
                return BadRequest();

            if (patches.Count == 0)
                return NoContent();

            var updated = await _experienceService.PatchExperiencesAsync(userId, patches);
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
