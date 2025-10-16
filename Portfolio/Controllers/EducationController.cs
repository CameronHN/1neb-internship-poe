using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

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
    }
}
