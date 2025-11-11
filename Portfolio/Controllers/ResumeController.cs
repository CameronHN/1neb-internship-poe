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

        [Authorize]
        [HttpPost("create-resume")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateResume(CreateResumeRequest request)
        {
            var userId = User.GetUserId()!.Value;

            // Prepare lists to collect new entity IDs
            Guid? summaryId = null;
            Guid? titleId = null;
            List<Guid> contactIds = new();
            List<Guid> skillIds = new();
            List<Guid> experienceIds = new();
            List<Guid> educationIds = new();
            List<Guid> certificationIds = new();

            // Use a transaction
            using (
                var transaction = await HttpContext
                    .RequestServices.GetRequiredService<ApplicationDbContext>()
                    .Database.BeginTransactionAsync()
            )
            {
                try
                {
                    // Add Title if provided
                    if (!string.IsNullOrWhiteSpace(request.Title))
                    {
                        var titleService =
                            HttpContext.RequestServices.GetRequiredService<ITitleService>();
                        var addTitle = new AddResumeTitle { Title = request.Title };
                        titleId = await titleService.AddTitleAsync(userId, addTitle);
                    }

                    // Add Contact Info if provided
                    if (request.Contacts != null && request.Contacts.Any())
                    {
                        var contactService =
                            HttpContext.RequestServices.GetRequiredService<IContactService>();
                        contactIds = await contactService.AddContactsAsync(
                            userId,
                            request.Contacts
                        );
                    }

                    // Add Professional Summary if provided
                    if (!string.IsNullOrWhiteSpace(request.Summary))
                    {
                        var summaryService =
                            HttpContext.RequestServices.GetRequiredService<IProfessionalSummaryService>();
                        var addSummary = new AddSummary { Summary = request.Summary };
                        summaryId = await summaryService.AddSummaryAsync(userId, addSummary);
                    }

                    // Add Skills if provided
                    if (request.Skills != null && request.Skills.Any())
                    {
                        var skillService =
                            HttpContext.RequestServices.GetRequiredService<ISkillService>();
                        skillIds = await skillService.AddSkillsAsync(userId, request.Skills);
                    }

                    // Add Experience if provided
                    if (request.Experiences != null && request.Experiences.Any())
                    {
                        var experienceService =
                            HttpContext.RequestServices.GetRequiredService<IExperienceService>();
                        experienceIds = await experienceService.AddExperiencesAsync(
                            userId,
                            request.Experiences
                        );
                    }

                    // Add Education if provided
                    if (request.Education != null && request.Education.Any())
                    {
                        var educationService =
                            HttpContext.RequestServices.GetRequiredService<IEducationService>();
                        educationIds = await educationService.AddEducationsAsync(
                            userId,
                            request.Education
                        );
                    }

                    // Add Certifications if provided
                    if (request.Certifications != null && request.Certifications.Any())
                    {
                        var certificationService =
                            HttpContext.RequestServices.GetRequiredService<ICertificationService>();
                        certificationIds = await certificationService.AddCertificationAsync(
                            userId,
                            request.Certifications
                        );
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // Map to ResumeRequest
            var resumeRequest = new ResumeRequest
            {
                TitleId = titleId,
                ProfessionalSummaryId = summaryId,
                SkillsIds = skillIds.Count != 0 ? new ItemListRequest { Ids = skillIds } : null,
                ExperienceIds =
                    experienceIds.Count != 0 ? new ItemListRequest { Ids = experienceIds } : null,
                EducationIds =
                    educationIds.Count != 0 ? new ItemListRequest { Ids = educationIds } : null,
                CertificationIds =
                    certificationIds.Count != 0
                        ? new ItemListRequest { Ids = certificationIds }
                        : null,
                SocialMediaIds =
                    contactIds.Count != 0 ? new ItemListRequest { Ids = contactIds } : null,
            };

            // Generate PDF using the above endpoint logic
            var userInfo = await _resumeService.GetResumeDetailsAsync(userId, resumeRequest);
            var pdf = _resumeService.RenderPdf(userInfo ?? new());

            if (pdf == null || pdf.Length == 0)
                return BadRequest("PDF generation failed.");

            string fileDownloadName = FileNameHelper.FileNameFormatter(userInfo?.Name);
            return File(pdf, "application/pdf", fileDownloadName);
        }
    }
}
