using System.ComponentModel.DataAnnotations;
using Portfolio.Core.Constants;
using Portfolio.Core.DTOs.Resume;
using Xunit;

namespace Portfolio.Tests.UnitTests
{
    /// <summary>
    /// The one place that decides the final section order (ResumeSections), and the request
    /// validation that rejects what Resolve would quietly ignore.
    /// </summary>
    public class ResumeSectionsTests
    {
        private static readonly string[] ClassicDefault =
        [
            ResumeSections.Experience,
            ResumeSections.Skills,
            ResumeSections.Education,
            ResumeSections.Certifications,
        ];

        private static readonly string[] SimplifiedDefault =
        [
            ResumeSections.Skills,
            ResumeSections.Experience,
            ResumeSections.Education,
            ResumeSections.Certifications,
        ];

        [Fact]
        public void Resolve_Null_ReturnsTheTemplateDefault()
        {
            Assert.Equal(ClassicDefault, ResumeSections.Resolve(null, ClassicDefault));
            Assert.Equal(SimplifiedDefault, ResumeSections.Resolve(null, SimplifiedDefault));
        }

        [Fact]
        public void Resolve_Empty_ReturnsTheTemplateDefault()
        {
            Assert.Equal(ClassicDefault, ResumeSections.Resolve([], ClassicDefault));
            Assert.Equal(SimplifiedDefault, ResumeSections.Resolve([], SimplifiedDefault));
        }

        [Fact]
        public void Resolve_PartialList_PutsListedFirstThenTheRestInDefaultOrder()
        {
            var classic = ResumeSections.Resolve(["certifications", "skills"], ClassicDefault);
            var simplified = ResumeSections.Resolve(["education"], SimplifiedDefault);

            Assert.Equal(["certifications", "skills", "experience", "education"], classic);
            Assert.Equal(["education", "skills", "experience", "certifications"], simplified);
        }

        [Fact]
        public void Resolve_FullList_IsUsedAsGiven()
        {
            var order = new[] { "certifications", "education", "skills", "experience" };

            Assert.Equal(order, ResumeSections.Resolve(order, ClassicDefault));
            Assert.Equal(order, ResumeSections.Resolve(order, SimplifiedDefault));
        }

        [Fact]
        public void Resolve_Duplicates_KeepsTheFirstOfEach()
        {
            var result = ResumeSections.Resolve(
                ["skills", "education", "skills", "education"],
                ClassicDefault
            );

            Assert.Equal(["skills", "education", "experience", "certifications"], result);
        }

        [Theory]
        [InlineData("summary")]
        [InlineData("title")]
        [InlineData("links")]
        [InlineData("professionalLinks")]
        [InlineData("")]
        [InlineData("   ")]
        public void Resolve_UnknownOrFixedKeys_AreDropped(string key)
        {
            var result = ResumeSections.Resolve([key, "education"], ClassicDefault);

            Assert.Equal(["education", "experience", "skills", "certifications"], result);
        }

        [Fact]
        public void Resolve_NullEntry_IsDropped()
        {
            var result = ResumeSections.Resolve([null, "education"], ClassicDefault);

            Assert.Equal(["education", "experience", "skills", "certifications"], result);
        }

        [Fact]
        public void Resolve_MixedCaseAndSpaces_AreNormalised()
        {
            var result = ResumeSections.Resolve(
                ["  CERTIFICATIONS ", "Skills", "sKiLLs"],
                ClassicDefault
            );

            Assert.Equal(["certifications", "skills", "experience", "education"], result);
        }

        [Fact]
        public void Clean_DoesNotAddTheSectionsThatWereLeftOut()
        {
            Assert.Equal(["skills"], ResumeSections.Clean([" SKILLS", "bogus", "skills"]));
            Assert.Empty(ResumeSections.Clean(null));
            Assert.Empty(ResumeSections.Clean(["summary"]));
        }

        [Fact]
        public void All_IsTheFourMovableSections()
        {
            Assert.Equal(ResumeSections.MaxCount, ResumeSections.All.Count);
            Assert.Equal(
                ["certifications", "education", "experience", "skills"],
                ResumeSections.All.Order()
            );
        }

        // --- request validation (model binding runs these attributes) ---

        [Theory]
        [InlineData("")]
        [InlineData("skills")]
        [InlineData("certifications,skills")]
        [InlineData("Experience, SKILLS ,education,CERTIFICATIONS")]
        public void ResumeDto_ValidSectionOrder_PassesValidation(string order)
        {
            var keys = order.Length == 0 ? [] : order.Split(',').ToList();

            var errors = Validate(new ResumeDTO { SectionOrder = keys });

            Assert.Empty(errors);
        }

        [Fact]
        public void ResumeDto_NullSectionOrder_PassesValidation()
        {
            Assert.Empty(Validate(new ResumeDTO { SectionOrder = null }));
        }

        [Theory]
        [InlineData("summary")]
        [InlineData("title")]
        [InlineData("links")]
        [InlineData("nonsense")]
        [InlineData("")]
        public void ResumeDto_UnknownKey_FailsValidation(string key)
        {
            var errors = Validate(new ResumeDTO { SectionOrder = ["skills", key] });

            var error = Assert.Single(errors);
            Assert.Contains(nameof(ResumeDTO.SectionOrder), error.MemberNames);
            Assert.Contains("not a section that can be ordered", error.ErrorMessage);
        }

        [Theory]
        [InlineData("skills", "skills")]
        [InlineData("skills", "SKILLS")]
        [InlineData("education", " Education ")]
        public void ResumeDto_DuplicateKey_FailsValidation(string first, string second)
        {
            var errors = Validate(new ResumeDTO { SectionOrder = [first, second] });

            var error = Assert.Single(errors);
            Assert.Contains("more than once", error.ErrorMessage);
        }

        [Fact]
        public void ResumeDto_MoreThanFourEntries_FailsValidation()
        {
            var errors = Validate(
                new ResumeDTO
                {
                    SectionOrder = ["skills", "experience", "education", "certifications", "skills"],
                }
            );

            Assert.Contains(errors, e => e.ErrorMessage == Portfolio.Core.Constants.Constants.MaxItemCountErrorMessage);
        }

        [Fact]
        public void ResumeRequest_UnknownKeyDuplicateOrTooMany_FailValidation()
        {
            Assert.NotEmpty(Validate(new ResumeRequest { SectionOrder = ["summary"] }));
            Assert.NotEmpty(Validate(new ResumeRequest { SectionOrder = ["skills", "skills"] }));
            Assert.NotEmpty(
                Validate(
                    new ResumeRequest
                    {
                        SectionOrder = ["skills", "experience", "education", "certifications", "x"],
                    }
                )
            );
            Assert.Empty(Validate(new ResumeRequest { SectionOrder = ["certifications", "Skills"] }));
            Assert.Empty(Validate(new ResumeRequest()));
        }

        private static List<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(
                model,
                new ValidationContext(model),
                results,
                validateAllProperties: true
            );
            return results;
        }
    }
}
