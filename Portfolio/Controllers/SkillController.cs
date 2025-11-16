using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.Skill;
using Portfolio.Core.Models;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddSkills([FromBody] List<AddSkill> skills)
        {
            var userId = User.GetUserId()!.Value;

            if (skills is null)
                throw new ValidationException("Request body cannot be null.");

            var skillIds = await _skillService.AddSkillsAsync(userId, skills);
            return Created(string.Empty, skillIds);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SkillModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSkillById([FromRoute] Guid id)
        {
            var skill = await _skillService.GetSkillByIdAsync(id);
            if (skill is null)
                return BadRequest();

            return Ok(skill);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchSkill([FromBody] PatchSkill patch)
        {
            var userId = User.GetUserId()!.Value;

            if (patch is null)
                throw new ValidationException("Request body cannot be null.");

            var updated = await _skillService.PatchSkillAsync(userId, patch);
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
            var userId = User.GetUserId()!.Value;

            if (skillIds == null || skillIds.Count == 0)
                return BadRequest("Skill IDs cannot be null or empty.");

            var deleted = await _skillService.DeleteSkillsAsync(userId, skillIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified skills.");

            return Ok(true);
        }

        [HttpGet("skills")]
        [ProducesResponseType(typeof(List<SkillsItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSkills()
        {
            var userId = User.GetUserId()!.Value;

            var skills = await _skillService.GetSkillsByUserIdAsync(userId);
            return Ok(skills);
        }
    }
}
