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
        public void Validate_ResumeMatchingDemoPageShape_Passes()
        {
            var exception = Record.Exception(() => DemoResumeValidator.Validate(DemoPageShape()));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_EmptyResume_Passes()
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
        public void Validate_OneMoreItemThanTheDemoPage_Throws(string section)
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
        /// The Demo page never sends sectionOrder, so any entry, even a template's default
        /// order, is a registered-user feature and gets a 403 (via ForbiddenException).
        /// </summary>
        [Theory]
        [InlineData("experience,skills,education,certifications")]
        [InlineData("skills,experience,education,certifications")]
        [InlineData("certifications")]
        [InlineData("skills")]
        public void Validate_AnySectionOrderEntries_ThrowsForbidden(string order)
        {
            var resume = DemoPageShape();
            resume.SectionOrder = [.. order.Split(',')];

            var exception = Assert.Throws<ForbiddenException>(() =>
                DemoResumeValidator.Validate(resume)
            );
            Assert.Equal("Section ordering is available to registered users.", exception.Message);
        }

        [Fact]
        public void Validate_NullOrEmptySectionOrder_Passes()
        {
            var nullOrder = DemoPageShape();
            nullOrder.SectionOrder = null;
            var emptyOrder = DemoPageShape();
            emptyOrder.SectionOrder = [];

            Assert.Null(Record.Exception(() => DemoResumeValidator.Validate(nullOrder)));
            Assert.Null(Record.Exception(() => DemoResumeValidator.Validate(emptyOrder)));
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
