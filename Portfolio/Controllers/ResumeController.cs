using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

namespace Portfolio.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;

        public ResumeController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        [HttpPost]
        [Produces("application/pdf")]
        public async Task<IActionResult> Generate([FromBody] ResumeDto dto)
        {
            var pdf = await _resumeService.RenderPdfAsync(dto ?? new());

            string? name = !string.IsNullOrEmpty(dto?.Name) ? dto.Name.Replace(' ', '_') + "_" : "";

            return File(pdf, "application/pdf", $"{name}resume.pdf");
        }

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(200, Type = typeof(ResumeDto))]
        public async Task<IActionResult> GetResume(Guid userId)
        {
            var resume = await _resumeService.GetResumeByUserId(userId);
            if (resume == null)
                return NotFound();

            return Ok(resume);
        }
    }
}
