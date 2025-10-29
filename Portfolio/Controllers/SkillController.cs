using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Skill;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpPost("skills")]
        [ProducesResponseType(typeof(List<SkillsItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSkillsByIds([FromBody] ItemListRequest request)
        {
            var skills = await _skillService.GetAllSkillsByIds(request);
            return Ok(skills);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddSkills([FromBody] List<AddSkill> skills)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var skillIds = await _skillService.AddSkillsAsync(userId.Value, skills);
            return Created(string.Empty, skillIds);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchSkills([FromBody] List<PatchSkill> patches)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _skillService.PatchSkillsAsync(userId.Value, patches);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSkills([FromBody] List<Guid> skillIds)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (skillIds == null || skillIds.Count == 0)
                return BadRequest("Skill IDs cannot be null or empty.");

            var deleted = await _skillService.DeleteSkillsAsync(userId.Value, skillIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified skills.");

            return Ok(true);
        }

        [HttpGet("skills")]
        [ProducesResponseType(typeof(List<SkillsItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSkills()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var skills = await _skillService.GetSkillsByUserIdAsync(userId.Value);
            return Ok(skills);
        }
    }
}
