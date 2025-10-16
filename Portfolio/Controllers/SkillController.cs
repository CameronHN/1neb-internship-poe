using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

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
    }
}
