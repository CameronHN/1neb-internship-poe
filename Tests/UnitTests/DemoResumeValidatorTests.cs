using Portfolio.Application.Validation;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Exceptions;
using Xunit;

namespace Portfolio.Tests.UnitTests
{
    /// <summary>
    /// The anonymous demo only accepts what the frontend's Demo page can send: 4 skills,
    /// 2 links, 2 experiences with 2 responsibilities each, 2 education and 2 certifications.
    /// </summary>
    public class DemoResumeValidatorTests
    {
        [Fact]
        public void DemoPageShape_Passes()
        {
            var exception = Record.Exception(() => DemoResumeValidator.Validate(DemoPageShape()));

            Assert.Null(exception);
        }

        [Fact]
        public void EmptyResume_Passes()
        {
            var exception = Record.Exception(() => DemoResumeValidator.Validate(new ResumeDTO()));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("skills")]
        [InlineData("links")]
        [InlineData("experiences")]
        [InlineData("responsibilities")]
        [InlineData("education")]
        [InlineData("certifications")]
        public void OneMoreThanTheDemoPage_Throws(string section)
        {
            var resume = DemoPageShape();
            switch (section)
            {
                case "skills":
                    resume.Skills!.Add(new SkillsItem());
                    break;
                case "links":
                    resume.ProfessionalLinks!.Add(new ProfessionalLinkItem());
                    break;
                case "experiences":
                    resume.Experience!.Add(new ExperienceItem());
                    break;
                case "responsibilities":
                    resume.Experience![1].Responsibilities!.Add("one too many");
                    break;
                case "education":
                    resume.Education!.Add(new EducationItem());
                    break;
                case "certifications":
                    resume.Certification!.Add(new CertificationItem());
                    break;
            }

            var exception = Assert.Throws<ValidationException>(() =>
                DemoResumeValidator.Validate(resume)
            );
            Assert.StartsWith("The demo allows at most", exception.Message);
        }

        /// <summary>
        /// The largest resume the Demo page can produce.
        /// </summary>
        private static ResumeDTO DemoPageShape()
        {
            return new ResumeDTO
            {
                Name = "Demo User",
                Skills = Enumerable.Range(0, 4).Select(_ => new SkillsItem { Skill = "C#" }).ToList(),
                ProfessionalLinks = Enumerable
                    .Range(0, 2)
                    .Select(_ => new ProfessionalLinkItem { Link = "https://example.com" })
                    .ToList(),
                Experience = Enumerable
                    .Range(0, 2)
                    .Select(_ => new ExperienceItem { Responsibilities = ["a", "b"] })
                    .ToList(),
                Education = Enumerable.Range(0, 2).Select(_ => new EducationItem()).ToList(),
                Certification = Enumerable.Range(0, 2).Select(_ => new CertificationItem()).ToList(),
            };
        }
    }
}
