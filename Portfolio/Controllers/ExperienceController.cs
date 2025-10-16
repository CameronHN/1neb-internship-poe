using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

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
        public async Task<IActionResult> GetAllExperiencesByIds([FromBody] List<Guid> ids)
        {
            var experiences = await _experienceService.GetAllExperiencesByIds(ids);
            return Ok(experiences);
        }
    }
}
