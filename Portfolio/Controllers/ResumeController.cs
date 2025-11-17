using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Resume;
using Portfolio.WebApi.Extensions;
using Portfolio.WebApi.Helper;

namespace Portfolio.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeDataService _resumeService;
        private readonly IResumeGenerationService _resumeGenerationService;

        public ResumeController(IResumeDataService resumeService, IResumeGenerationService resumeGenerationService)
        {
            _resumeService = resumeService;
            _resumeGenerationService = resumeGenerationService;
        }

        [AllowAnonymous]
        [HttpPost("create-pdf")]
        public async Task<IActionResult> GenerateResumeWithoutSavingDetails([FromBody] ResumeDto dto)
        {
            var pdf = await _resumeGenerationService.GenerateResumePdfAsync(dto);

            string? name = !string.IsNullOrEmpty(dto?.Name) ? dto.Name.Replace(' ', '_') + "_" : "";

            return File(pdf, "application/pdf", $"{name}resume.pdf");
        }

        [Authorize]
        [HttpGet("get-user-resume-details")]
        [ProducesResponseType(200, Type = typeof(ResumeDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllResumeDetailsByUserId()
        {
            var userId = User.GetUserId()!.Value;

            var resume = await _resumeService.GetResumeByUserId(userId);
            if (resume == null)
                return NotFound();

            return Ok(resume);
        }

        [Authorize]
        [HttpPost("get-resume")]
        public async Task<IActionResult> GenerateResumeByIds(ResumeRequest resumeRequest)
        {
            var userId = User.GetUserId()!.Value;

            var userInfo = await _resumeService.GetResumeDetailsAsync(userId, resumeRequest);
            var pdf = await _resumeGenerationService.GenerateResumePdfAsync(userInfo);

            if (pdf == null || pdf.Length == 0)
                return BadRequest("PDF generation failed.");

            string fileDownloadName = FileNameHelper.FileNameFormatter(userInfo?.Name);
            return File(pdf, "application/pdf", fileDownloadName);
        }
    }
}
