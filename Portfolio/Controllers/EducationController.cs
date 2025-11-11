using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _educationService;

        public EducationController(IEducationService educationService)
        {
            _educationService = educationService;
        }

        [HttpPost("educations")]
        [ProducesResponseType(typeof(List<EducationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEducationsByIds([FromBody] ItemListRequest request)
        {
            var educations = await _educationService.GetAllEducationsByIds(request);
            return Ok(educations);
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteEducations([FromBody] List<Guid> educationIds)
        {
            var userId = User.GetUserId()!.Value;

            if (educationIds == null || educationIds.Count == 0)
                return BadRequest("Education IDs cannot be null or empty.");

            var deleted = await _educationService.DeleteEducationsAsync(userId, educationIds);
            if (!deleted)
                return BadRequest("Failed to delete the specified educations.");

            return Ok(true);
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddEducations([FromBody] List<AddEducation> educations)
        {
            var userId = User.GetUserId()!.Value;

            var educationIds = await _educationService.AddEducationsAsync(userId, educations);
            return Created(string.Empty, educationIds);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EducationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEducationById([FromRoute] Guid id)
        {
            var title = await _educationService.GetEducationByIdAsync(id);
            if (title is null)
                return BadRequest();

            return Ok(title);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchEducations([FromBody] List<PatchEducation> patches)
        {
            var userId = User.GetUserId()!.Value;

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _educationService.PatchEducationsAsync(userId, patches);
            if (!updated)
                return NoContent();

            return Ok(true);
        }

        [HttpGet("educations")]
        [ProducesResponseType(typeof(List<EducationItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEducations()
        {
            var userId = User.GetUserId()!.Value;

            var educations = await _educationService.GetEducationsByUserIdAsync(userId);
            return Ok(educations);
        }
    }
}
