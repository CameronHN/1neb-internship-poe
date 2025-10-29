using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.DTOs.Certification;
using Portfolio.Core.DTOs.Contact;
using Portfolio.Core.DTOs.Education;
using Portfolio.Core.DTOs.Experience;
using Portfolio.Core.DTOs.Experience.ExperienceResponsibility;
using Portfolio.Core.DTOs.ProfessionalSummary;
using Portfolio.Core.DTOs.ResumeTitle;
using Portfolio.Core.DTOs.Skill;
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

        [HttpPost("create-pdf")]
        public IActionResult GenerateResumeWithoutSavingDetails([FromBody] ResumeDto dto)
        {
            var pdf = _resumeService.RenderPdf(dto ?? new());

            string? name = !string.IsNullOrEmpty(dto?.Name) ? dto.Name.Replace(' ', '_') + "_" : "";

            return File(pdf, "application/pdf", $"{name}resume.pdf");
        }

        [HttpGet("get-user-resume-details")]
        [ProducesResponseType(200, Type = typeof(ResumeDto))]
        public async Task<IActionResult> GetResumeDtoByUserId()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var resume = await _resumeService.GetResumeByUserId(userId.Value);
            if (resume == null)
                return NotFound();

            return Ok(resume);
        }

        [Authorize]
        [HttpPost("get-user-resume")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerateResumeByUserID()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var userInfo = await _resumeService.GetResumeByUserId(userId.Value);
            if (userInfo == null)
                return NotFound("Resume details not found for the user.");

            var pdf = _resumeService.RenderPdf(userInfo ?? new());

            string fileDownloadName = FileNameHelper.FileNameFormatter(userInfo?.Name);
            return File(pdf, "application/pdf", fileDownloadName);
        }

        [HttpPost("get-resume")]
        [Produces("application/pdf")]
        public async Task<IActionResult> GenerateResumeByIds(ResumeRequest resumeRequest)
        {
            var userInfo = await _resumeService.GetResumeDetailsAsync(resumeRequest);
            var pdf = _resumeService.RenderPdf(userInfo ?? new());

            if (pdf == null || pdf.Length == 0)
                return BadRequest("PDF generation failed.");

            string fileDownloadName = FileNameHelper.FileNameFormatter(userInfo?.Name);
            return File(pdf, "application/pdf", fileDownloadName);
        }

        [HttpPost("create-resume")]
        public async Task<IActionResult> CreateResume(ResumeDto resumeDto)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

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
                    // --- Get User ID ---
                    Guid id = userId.Value;

                    // Add Title if provided
                    if (!string.IsNullOrWhiteSpace(resumeDto.Title))
                    {
                        var titleService =
                            HttpContext.RequestServices.GetRequiredService<ITitleService>();
                        var addTitle = new AddResumeTitle { Title = resumeDto.Title };
                        titleId = await titleService.AddTitleAsync(id, addTitle);
                    }

                    // Add Contact Info if provided
                    if (resumeDto.Socials != null && resumeDto.Socials.Any())
                    {
                        var contactService =
                            HttpContext.RequestServices.GetRequiredService<IContactService>();

                        var addContacts = new List<AddContact>();

                        if (resumeDto.Socials != null && resumeDto.Socials.Any())
                        {
                            addContacts.AddRange(
                                resumeDto
                                    .Socials.Where(s =>
                                        !string.IsNullOrWhiteSpace(s.SocialMediaUrl)
                                    )
                                    .Select(s => new AddContact { Social = s.SocialMediaUrl })
                                    .ToList()
                            );
                        }

                        contactIds = await contactService.AddContactsAsync(id, addContacts);
                    }

                    // Add Professional Summary if provided
                    if (!string.IsNullOrWhiteSpace(resumeDto.Summary))
                    {
                        var summaryService =
                            HttpContext.RequestServices.GetRequiredService<IProfessionalSummaryService>();
                        var addSummary = new AddSummary { Summary = resumeDto.Summary };

                        summaryId = await summaryService.AddSummaryAsync(id, addSummary);
                    }

                    // Add Skills if provided
                    if (resumeDto.Skills != null && resumeDto.Skills.Any())
                    {
                        var skillService =
                            HttpContext.RequestServices.GetRequiredService<ISkillService>();
                        var addSkills = resumeDto
                            .Skills.Select(s => new AddSkill
                            {
                                Skill = s.Skill,
                                ProficiencyLevel = s.SkillLevel,
                            })
                            .ToList();

                        skillIds = await skillService.AddSkillsAsync(id, addSkills);
                    }

                    // Add Experience if provided
                    if (resumeDto.Experience != null && resumeDto.Experience.Any())
                    {
                        var experienceService =
                            HttpContext.RequestServices.GetRequiredService<IExperienceService>();
                        var addExperiences = resumeDto
                            .Experience.Select(e => new AddExperience
                            {
                                JobTitle = e.JobTitle,
                                CompanyName = e.Company,
                                StartDate = e.StartDate,
                                EndDate = e.EndDate,
                                Responsibilities = e
                                    .Responsibilities.Select(r => new AddResponsibility
                                    {
                                        Responsibility = r,
                                    })
                                    .ToList(),
                            })
                            .ToList();

                        experienceIds = await experienceService.AddExperiencesAsync(
                            id,
                            addExperiences
                        );
                    }

                    // Add Education if provided
                    if (resumeDto.Education != null && resumeDto.Education.Any())
                    {
                        var educationService =
                            HttpContext.RequestServices.GetRequiredService<IEducationService>();
                        var addEducations = resumeDto
                            .Education.Select(e => new AddEducation
                            {
                                InstitutionName = e.Institution,
                                Qualification = e.Qualification,
                                Major = e.Major,
                                StartDate = e.StartDate,
                                EndDate = e.EndDate,
                            })
                            .ToList();

                        educationIds = await educationService.AddEducationsAsync(id, addEducations);
                    }

                    // Add Certifications if provided
                    if (resumeDto.Certification != null && resumeDto.Certification.Any())
                    {
                        var certificationService =
                            HttpContext.RequestServices.GetRequiredService<ICertificationService>();
                        var addCerts = resumeDto
                            .Certification.Select(c => new AddCertification
                            {
                                CertificationName = c.Name,
                                IssuingOrganisation = c.Organisation,
                                CredentialUrl = c.CredentialUrl,
                                IssuedDate = c.IssuedDate,
                                ExpiryDate = c.ExpirationDate,
                            })
                            .ToList();

                        certificationIds = await certificationService.AddCertificationAsync(
                            id,
                            addCerts
                        );
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Failed to build resume. No data was saved.");
                }
            }

            // Map to ResumeRequest
            var resumeRequest = new ResumeRequest
            {
                UserId = userId.Value,
                TitleId = titleId,
                ProfessionalSummaryId = summaryId,
                SkillsIds = skillIds.Any() ? new ItemListRequest { Ids = skillIds } : null,
                ExperienceIds = experienceIds.Any()
                    ? new ItemListRequest { Ids = experienceIds }
                    : null,
                EducationIds = educationIds.Any()
                    ? new ItemListRequest { Ids = educationIds }
                    : null,
                CertificationIds = certificationIds.Any()
                    ? new ItemListRequest { Ids = certificationIds }
                    : null,
                SocialMediaIds = contactIds.Any() ? new ItemListRequest { Ids = contactIds } : null,
            };

            // Generate PDF using the above endpoint logic
            var userInfo = await _resumeService.GetResumeDetailsAsync(resumeRequest);
            var pdf = _resumeService.RenderPdf(userInfo ?? new());

            if (pdf == null || pdf.Length == 0)
                return BadRequest("PDF generation failed.");

            string fileDownloadName = FileNameHelper.FileNameFormatter(userInfo?.Name);
            return File(pdf, "application/pdf", fileDownloadName);
        }
    }
}
