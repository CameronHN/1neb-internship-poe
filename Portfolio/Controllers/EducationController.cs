using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Education;
using Portfolio.WebApi.Extensions;

namespace Portfolio.WebApi.Controllers
{
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

        [HttpPost("add")]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddEducations([FromBody] List<AddEducation> educations)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            foreach (var education in educations)
            {
                education.UserId = userId.Value;
            }

            var educationIds = await _educationService.AddEducationsAsync(educations);
            return Created(string.Empty, educationIds);
        }

        [HttpPatch("patch")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PatchEducations([FromBody] List<PatchEducation> patches)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            if (patches is null)
                throw new ValidationException("Request body cannot be null.");

            if (patches.Count == 0)
                return NoContent();

            var updated = await _educationService.PatchEducationsAsync(userId.Value, patches);
            if (!updated)
                return NoContent();

            return Ok(true);
        }
    }
}
