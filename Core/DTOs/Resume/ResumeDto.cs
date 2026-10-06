using System.ComponentModel.DataAnnotations;
using Portfolio.Core.Constants;
using static Portfolio.Core.Constants.Constants;

namespace Portfolio.Core.DTOs.Resume
{
    // The caps below only apply when a ResumeDTO is model-bound from a request body.
    // Saved snapshots are deserialised without validation, so older saved resumes still load.
    public class ResumeDTO
    {
        [MaxLength(ResumeLimits.NameLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Name { get; set; }

        [MaxLength(ResumeLimits.TitleLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Title { get; set; }

        [MaxLength(ResumeLimits.EmailLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Email { get; set; }

        [MaxLength(ResumeLimits.PhoneNumberLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? PhoneNumber { get; set; }

        [MaxLength(ResumeLimits.SummaryLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Summary { get; set; }

        [MaxLength(ResumeLimits.MaxSkills, ErrorMessage = MaxItemCountErrorMessage)]
        public List<SkillsItem>? Skills { get; set; }

        [MaxLength(ResumeLimits.MaxProfessionalLinks, ErrorMessage = MaxItemCountErrorMessage)]
        public List<ProfessionalLinkItem>? ProfessionalLinks { get; set; }

        [MaxLength(ResumeLimits.MaxExperiences, ErrorMessage = MaxItemCountErrorMessage)]
        public List<ExperienceItem>? Experience { get; set; }

        [MaxLength(ResumeLimits.MaxEducation, ErrorMessage = MaxItemCountErrorMessage)]
        public List<EducationItem>? Education { get; set; }

        [MaxLength(ResumeLimits.MaxCertifications, ErrorMessage = MaxItemCountErrorMessage)]
        public List<CertificationItem>? Certification { get; set; }
    }

    public class ProfessionalLinkItem
    {
        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Link { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? LinkType { get; set; }
    }

    public class SkillsItem
    {
        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Skill { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? SkillLevel { get; set; }
    }

    public class ExperienceItem : IValidatableObject
    {
        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Company { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? JobTitle { get; set; }

        [MaxLength(ResumeLimits.DateLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? StartDate { get; set; }

        [MaxLength(ResumeLimits.DateLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? EndDate { get; set; }

        [MaxLength(
            ResumeLimits.MaxResponsibilitiesPerExperience,
            ErrorMessage = MaxItemCountErrorMessage
        )]
        public List<string>? Responsibilities { get; set; }

        // [MaxLength] on a List<string> checks the item count, not each string's length.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (
                Responsibilities?.Any(r => r?.Length > ResumeLimits.ResponsibilityLength)
                == true
            )
            {
                yield return new ValidationResult(
                    MaxCharacterLengthErrorMessage,
                    [nameof(Responsibilities)]
                );
            }
        }
    }

    public class EducationItem
    {
        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Institution { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Qualification { get; set; }

        [MaxLength(ResumeLimits.DateLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? StartDate { get; set; }

        [MaxLength(ResumeLimits.DateLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? EndDate { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Major { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Achievement { get; set; }
    }

    public class CertificationItem
    {
        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Name { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? Organisation { get; set; }

        [MaxLength(ResumeLimits.TextLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? CredentialUrl { get; set; }

        [MaxLength(ResumeLimits.DateLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? IssuedDate { get; set; }

        [MaxLength(ResumeLimits.DateLength, ErrorMessage = MaxCharacterLengthErrorMessage)]
        public string? ExpirationDate { get; set; }
    }
}
