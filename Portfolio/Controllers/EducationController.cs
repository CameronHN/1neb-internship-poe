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
    }
}
