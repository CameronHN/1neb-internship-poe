using System.ComponentModel.DataAnnotations;
using Portfolio.Core.Constants;
using Portfolio.Core.DTOs.Resume;
using Xunit;

namespace Portfolio.Tests.UnitTests
{
    /// <summary>
    /// The one place that decides the final section order (ResumeSections), and the request
    /// validation that rejects what Resolve would quietly ignore. The API and stored JSON use
    /// lower-case string keys; ResumeSections turns them into the ResumeSection enum.
    /// </summary>
    public class ResumeSectionsTests
    {
        private static readonly ResumeSection[] ClassicDefault =
        [
            ResumeSection.Experience,
            ResumeSection.Skills,
            ResumeSection.Education,
            ResumeSection.Certifications,
        ];

        private static readonly ResumeSection[] SimplifiedDefault =
        [
            ResumeSection.Skills,
            ResumeSection.Experience,
            ResumeSection.Education,
            ResumeSection.Certifications,
        ];

        // Resolve returns enum values; the tests compare them as the keys the API uses.
        private static List<string> Keys(IEnumerable<ResumeSection> sections)
        {
            return ResumeSections.ToKeys(sections);
        }

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

            Assert.Equal(["certifications", "skills", "experience", "education"], Keys(classic));
            Assert.Equal(["education", "skills", "experience", "certifications"], Keys(simplified));
        }

        [Fact]
        public void Resolve_FullList_IsUsedAsGiven()
        {
            var order = new[] { "certifications", "education", "skills", "experience" };

            Assert.Equal(order, Keys(ResumeSections.Resolve(order, ClassicDefault)));
            Assert.Equal(order, Keys(ResumeSections.Resolve(order, SimplifiedDefault)));
        }

        [Fact]
        public void Resolve_Duplicates_KeepsTheFirstOfEach()
        {
            var result = ResumeSections.Resolve(
                ["skills", "education", "skills", "education"],
                ClassicDefault
            );

            Assert.Equal(["skills", "education", "experience", "certifications"], Keys(result));
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

            Assert.Equal(["education", "experience", "skills", "certifications"], Keys(result));
        }

        /// <summary>
        /// Enum.TryParse would accept all of these ("1" is Experience, "Skills, Education" is a
        /// flags-style combination, "7" is not even a member), so keys are matched by name only.
        /// </summary>
        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("7")]
        [InlineData("-1")]
        [InlineData("skills,education")]
        [InlineData("Skills, Education")]
        [InlineData("skills|education")]
        public void Resolve_NumbersAndCombinedValues_AreDropped(string key)
        {
            var result = ResumeSections.Resolve([key], ClassicDefault);

            Assert.Equal(ClassicDefault, result);
            Assert.False(ResumeSections.IsKnown(key));
        }

        [Fact]
        public void Resolve_NullEntry_IsDropped()
        {
            var result = ResumeSections.Resolve([null, "education"], ClassicDefault);

            Assert.Equal(["education", "experience", "skills", "certifications"], Keys(result));
        }

        [Fact]
        public void Resolve_MixedCaseAndSpaces_AreNormalised()
        {
            var result = ResumeSections.Resolve(
                ["  CERTIFICATIONS ", "Skills", "sKiLLs"],
                ClassicDefault
            );

            Assert.Equal(["certifications", "skills", "experience", "education"], Keys(result));
        }

        [Fact]
        public void Clean_DoesNotAddTheSectionsThatWereLeftOut()
        {
            Assert.Equal([ResumeSection.Skills], ResumeSections.Clean([" SKILLS", "bogus", "skills"]));
            Assert.Empty(ResumeSections.Clean(null));
            Assert.Empty(ResumeSections.Clean(["summary"]));
        }

        [Fact]
        public void All_IsTheFourMovableSections()
        {
            Assert.Equal(ResumeSections.MaxCount, ResumeSections.All.Count);
            Assert.Equal(
                ["certifications", "education", "experience", "skills"],
                Keys(ResumeSections.All).Order()
            );
        }

        /// <summary>
        /// The key is the enum member name in lower case. It is what the API accepts and what
        /// saved snapshots store, so renaming a member would break old snapshots.
        /// </summary>
        [Fact]
        public void ToKey_AndTryParse_RoundTripEverySectionAsLowerCaseNames()
        {
            foreach (var section in ResumeSections.All)
            {
                var key = ResumeSections.ToKey(section);

                Assert.Equal(section.ToString().ToLowerInvariant(), key);
                Assert.True(ResumeSections.TryParse(key, out var parsed));
                Assert.Equal(section, parsed);
                Assert.True(ResumeSections.TryParse(key.ToUpperInvariant(), out var upper));
                Assert.Equal(section, upper);
            }

            Assert.Equal(
                ["skills", "experience", "education", "certifications"],
                Keys(ResumeSections.All)
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
        [InlineData("1")]
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
