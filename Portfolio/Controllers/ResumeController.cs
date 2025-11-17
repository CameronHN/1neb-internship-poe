using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.ProfessionalSummary;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.DTOs.ResumeTitle;
using Portfolio.Infrastructure.Persistence;
using Portfolio.WebApi.Extensions;
using Portfolio.WebApi.Helper;

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

        [AllowAnonymous]
        [HttpPost("create-pdf")]
        public IActionResult GenerateResumeWithoutSavingDetails([FromBody] ResumeDto dto)
        {
            var pdf = _resumeService.RenderPdf(dto ?? new());

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
        [Produces("application/pdf")]
        public async Task<IActionResult> GenerateResumeByIds(ResumeRequest resumeRequest)
        {
            var userId = User.GetUserId()!.Value;

            var userInfo = await _resumeService.GetResumeDetailsAsync(userId, resumeRequest);
            var pdf = _resumeService.RenderPdf(userInfo ?? new());

            if (pdf == null || pdf.Length == 0)
                return BadRequest("PDF generation failed.");

            string fileDownloadName = FileNameHelper.FileNameFormatter(userInfo?.Name);
            return File(pdf, "application/pdf", fileDownloadName);
        }
    }
}
