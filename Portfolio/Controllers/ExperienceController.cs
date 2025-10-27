using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Experience;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
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

        [HttpGet("experiences/{userId}")]
        [ProducesResponseType(typeof(List<ExperienceItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExperienceItemsByUserId(Guid userId)
        {
            var experiences = await _experienceService.GetExperienceItemsByUserId(userId);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddExperiences([FromBody] List<AddExperience> experiences)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            foreach (var experience in experiences)
            {
                experience.UserId = userId.Value;
            }

            var experienceIds = await _experienceService.AddExperiencesAsync(experiences);
            return Created(string.Empty, experienceIds);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchExperiences([FromBody] List<PatchExperience> patches)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _experienceService.PatchExperiencesAsync(userId.Value, patches);
            if (!updated)
                return NoContent();

            return Ok(true);
        }
    }
}
