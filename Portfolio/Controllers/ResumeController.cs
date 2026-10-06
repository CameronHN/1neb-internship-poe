using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Application.Validation;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.Resume;
using Portfolio.WebApi.Extensions;
using Portfolio.WebApi.Helper;
using Portfolio.WebApi.RateLimiting;

namespace Portfolio.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeDataService _resumeDataService;
        private readonly IResumeGenerationService _resumeGenerationService;

        public ResumeController(
            IResumeDataService resumeDataService,
            IResumeGenerationService resumeGenerationService
        )
        {
            _resumeDataService = resumeDataService;
            _resumeGenerationService = resumeGenerationService;
        }

        /// <summary>
        /// The anonymous demo. The server enforces its quota per client IP (see DemoQuota and
        /// the "RateLimiting:Demo" settings) and only accepts what the Demo page can send.
        /// </summary>
        [AllowAnonymous]
        [DemoQuota]
        [PdfGeneration]
        [RequestSizeLimit(32 * 1024)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("create-pdf")]
        public async Task<IActionResult> GenerateResumeWithoutSavingDetails(
            [FromBody] ResumeDTO dto
        )
        {
            DemoResumeValidator.Validate(dto);

            var pdf = await _resumeGenerationService.GenerateResumePdfAsync(dto);

            string? name = FileNameHelper.FileNameFormatter(dto?.Name);

            return File(pdf, "application/pdf", $"{name}resume.pdf");
        }

        [Authorize]
        [HttpGet("get-user-resume-details")]
        [ProducesResponseType(200, Type = typeof(GetAllResumeDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllResumeDetailsByUserId()
        {
            var userId = User.GetUserId()!.Value;

            var resume = await _resumeDataService.GetResumeByUserIdAsync(userId);
            if (resume == null)
                return NotFound();

            return Ok(resume);
        }

        [Authorize]
        [EnableRateLimiting(RateLimitPolicies.Pdf)]
        [PdfGeneration]
        [RequestSizeLimit(256 * 1024)]
        [HttpPost("get-resume")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateResumeByIds(ResumeRequest resumeRequest)
        {
            var userId = User.GetUserId()!.Value;

            var userInfo = await _resumeDataService.GetResumeDetailsAsync(userId, resumeRequest);
            var pdf = await _resumeGenerationService.GenerateResumePdfAsync(userInfo);

            if (pdf == null || pdf.Length == 0)
                return BadRequest("PDF generation failed.");

            string fileDownloadName = FileNameHelper.FileNameFormatter(userInfo?.Name);
            return File(pdf, "application/pdf", fileDownloadName);
        }
    }
}
