using Portfolio.Core.Constants;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Exceptions;

namespace Portfolio.Application.Validation
{
    /// <summary>
    /// Checks that a resume sent to the anonymous demo endpoint is no bigger than the
    /// frontend's Demo page can produce. String lengths are already checked by the
    /// attributes on ResumeDTO; this adds the much smaller item counts.
    /// </summary>
    public static class DemoResumeValidator
    {
        public static void Validate(ResumeDTO resume)
        {
            CheckCount(resume.Skills?.Count, DemoResumeLimits.MaxSkills, "skills");
            CheckCount(
                resume.ProfessionalLinks?.Count,
                DemoResumeLimits.MaxProfessionalLinks,
                "professional links"
            );
            CheckCount(resume.Experience?.Count, DemoResumeLimits.MaxExperiences, "experiences");
            CheckCount(resume.Education?.Count, DemoResumeLimits.MaxEducation, "education entries");
            CheckCount(
                resume.Certification?.Count,
                DemoResumeLimits.MaxCertifications,
                "certifications"
            );

            foreach (var experience in resume.Experience ?? [])
            {
                CheckCount(
                    experience?.Responsibilities?.Count,
                    DemoResumeLimits.MaxResponsibilitiesPerExperience,
                    "responsibilities per experience"
                );
            }
        }

        private static void CheckCount(int? count, int max, string items)
        {
            if (count > max)
            {
                throw new ValidationException($"The demo allows at most {max} {items}.");
            }
        }
    }
}
