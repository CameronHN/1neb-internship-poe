using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
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

        //[HttpPost]
        //[Produces("application/pdf")]
        //public async Task<IActionResult> Generate([FromBody] ResumeDto dto)
        //{
        //    var pdf = await _resumeService.RenderPdfAsync(dto ?? new());

        //    string? name = !string.IsNullOrEmpty(dto?.Name) ? dto.Name.Replace(' ', '_') + "_" : "";

        //    return File(pdf, "application/pdf", $"{name}resume.pdf");
        //}

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(200, Type = typeof(ResumeDto))]
        public async Task<IActionResult> GetResumeDtoByUserId(Guid userId)
        {
            var resume = await _resumeService.GetResumeByUserId(userId);
            if (resume == null)
                return NotFound();

            return Ok(resume);
        }

        [Authorize]
        [HttpPost("user/pdf")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GeneratePdfByUserID()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var userInfo = await _resumeService.GetResumeByUserId(userId.Value);

            var pdf = _resumeService.RenderPdf(userInfo ?? new());

            return File(pdf, "application/pdf", FileNameHelper.FileNameFormatter(userInfo?.Name));
        }

        [HttpPost("get-resume")]
        [Produces("application/pdf")]
        public async Task<IActionResult> GenerateResumeByIds(ResumeRequest resumeRequest)
        {
            try
            {
                var userInfo = await _resumeService.GetResume(resumeRequest);
                var pdf = _resumeService.RenderPdf(userInfo ?? new());

                if (pdf == null || pdf.Length == 0)
                    return BadRequest("PDF generation failed.");

                return File(
                    pdf,
                    "application/pdf",
                    FileNameHelper.FileNameFormatter(userInfo?.Name));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while generating the PDF.");
            }
        }
    }
}
