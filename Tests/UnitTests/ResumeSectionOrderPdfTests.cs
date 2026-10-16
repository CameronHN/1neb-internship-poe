using Portfolio.Application.Documents;
using Portfolio.Core.Constants;
using Portfolio.Core.DTOs.Resume;
using Portfolio.Tests.Common;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Xunit;

namespace Portfolio.Tests.UnitTests
{
    /// <summary>
    /// Renders real PDFs from both generators and reads the result back with PdfPig, so the
    /// order, the skipped sections and the Simplified dividers are checked on the output
    /// and not on the code.
    /// </summary>
    public class ResumeSectionOrderPdfTests
    {
        // Upper-case titles as drawn (whitespace is stripped from extracted text), and the first
        // word of each title so its position can be looked up.
        private static readonly Dictionary<string, (string Text, string FirstWord)> ClassicTitles = new()
        {
            [ResumeSections.Skills] = ("SKILLS", "SKILLS"),
            [ResumeSections.Experience] = ("EXPERIENCE", "EXPERIENCE"),
            [ResumeSections.Education] = ("EDUCATION", "EDUCATION"),
            [ResumeSections.Certifications] = ("CERTIFICATIONS", "CERTIFICATIONS"),
        };

        private static readonly Dictionary<string, (string Text, string FirstWord)> SimplifiedTitles = new()
        {
            [ResumeSections.Skills] = ("SKILLS", "SKILLS"),
            [ResumeSections.Experience] = ("PROFESSIONALEXPERIENCE", "PROFESSIONAL"),
            [ResumeSections.Education] = ("EDUCATION", "EDUCATION"),
            [ResumeSections.Certifications] = ("CERTIFICATIONS", "CERTIFICATIONS"),
        };

        private static readonly string[] ClassicDefault = ["experience", "skills", "education", "certifications"];
        private static readonly string[] SimplifiedDefault = ["skills", "experience", "education", "certifications"];

        public ResumeSectionOrderPdfTests()
        {
            // Program.cs sets this at startup. Unit tests do not run Program.cs, so set it here.
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Both templates crossed with all 24 orders of the four sections.
        /// </summary>
        public static TheoryData<string, string> EveryTemplateAndOrder()
        {
            var data = new TheoryData<string, string>();
            foreach (var template in new[] { TemplateTypes.Classic, TemplateTypes.Simplified })
            {
                foreach (var order in Permutations(ResumeSections.All.ToList()))
                {
                    data.Add(template, string.Join(",", order));
                }
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(EveryTemplateAndOrder))]
        public void Pdf_AnyOrderOfTheFourSections_DrawsThemInThatOrderAfterTheHeaderAndSummary(
            string template,
            string order
        )
        {
            var requested = order.Split(',');

            var pdf = Render(template, FullResume(requested));

            var titles = Titles(template);
            Assert.Equal(requested.Select(key => titles[key].Text), HeadingsInOrder(pdf, template));

            // Header (name) first, then the Summary, then the first movable section.
            var text = PdfText.ExtractWithoutWhitespace(pdf);
            var firstSection = text.IndexOf(titles[requested[0]].Text, StringComparison.Ordinal);
            Assert.True(text.IndexOf("JaneTester", StringComparison.Ordinal) < text.IndexOf("SUMMARY", StringComparison.Ordinal));
            Assert.True(text.IndexOf("SUMMARY", StringComparison.Ordinal) < firstSection);
        }

        [Theory]
        [InlineData(TemplateTypes.Classic)]
        [InlineData(TemplateTypes.Simplified)]
        public void Pdf_NoSectionOrder_UsesTheTemplatesDefaultOrder(string template)
        {
            var expected = template == TemplateTypes.Classic ? ClassicDefault : SimplifiedDefault;

            foreach (List<string>? order in new List<string>?[] { null, [] })
            {
                var pdf = Render(template, FullResume(order));

                Assert.Equal(
                    expected.Select(key => Titles(template)[key].Text),
                    HeadingsInOrder(pdf, template)
                );
            }
        }

        [Theory]
        [InlineData(TemplateTypes.Classic, "certifications,skills", "certifications,skills,experience,education")]
        [InlineData(TemplateTypes.Simplified, "certifications,skills", "certifications,skills,experience,education")]
        [InlineData(TemplateTypes.Classic, "education", "education,experience,skills,certifications")]
        [InlineData(TemplateTypes.Simplified, "education", "education,skills,experience,certifications")]
        public void Pdf_PartialOrder_ListedSectionsFirstThenTheRestInDefaultOrder(
            string template,
            string requested,
            string expected
        )
        {
            var pdf = Render(template, FullResume([.. requested.Split(',')]));

            Assert.Equal(
                expected.Split(',').Select(key => Titles(template)[key].Text),
                HeadingsInOrder(pdf, template)
            );
        }

        /// <summary>
        /// Saved snapshots are not validated when they are read, so a bad stored order must be
        /// ignored and not throw.
        /// </summary>
        [Theory]
        [InlineData(TemplateTypes.Classic)]
        [InlineData(TemplateTypes.Simplified)]
        public void Pdf_StoredOrderWithUnknownKeysAndDuplicates_IgnoresTheBadEntries(string template)
        {
            var pdf = Render(
                template,
                FullResume(["summary", "Education", "title", "education", "links", "bogus", null!, "SKILLS"])
            );

            // Education and Skills are the valid entries. Experience and Certifications follow in
            // default order, which is the same for both templates once those two are placed.
            string[] expected = ["education", "skills", "experience", "certifications"];
            Assert.Equal(
                expected.Select(key => Titles(template)[key].Text),
                HeadingsInOrder(pdf, template)
            );
        }

        /// <summary>
        /// A section with no data is not drawn in any order (16 combinations of empty and
        /// populated, each in reverse default order), and Simplified gets exactly one divider
        /// for the header and one above each drawn section: none doubled, leading or trailing.
        /// </summary>
        [Theory]
        [InlineData(TemplateTypes.Classic)]
        [InlineData(TemplateTypes.Simplified)]
        public void Pdf_SectionsWithoutData_AreSkippedWithoutExtraDividers(string template)
        {
            var titles = Titles(template);
            var order = (template == TemplateTypes.Classic ? ClassicDefault : SimplifiedDefault)
                .Reverse()
                .ToList();

            for (var mask = 0; mask < 16; mask++)
            {
                var populated = order.Where((_, i) => (mask & (1 << i)) != 0).ToList();
                var resume = FullResume(order);
                if (!populated.Contains(ResumeSections.Skills)) resume.Skills = null;
                if (!populated.Contains(ResumeSections.Experience)) resume.Experience = [];
                if (!populated.Contains(ResumeSections.Education)) resume.Education = null;
                if (!populated.Contains(ResumeSections.Certifications)) resume.Certification = [];

                var pdf = Render(template, resume);

                Assert.Equal(
                    populated.Select(key => titles[key].Text),
                    HeadingsInOrder(pdf, template));
                if (template == TemplateTypes.Simplified)
                {
                    AssertOneDividerAboveEachSection(pdf, populated.Select(key => titles[key].FirstWord).ToList());
                }
            }
        }

        public static TheoryData<string> EveryOrder()
        {
            var data = new TheoryData<string>();
            foreach (var order in Permutations(ResumeSections.All.ToList()))
            {
                data.Add(string.Join(",", order));
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(EveryOrder))]
        public void SimplifiedPdf_AnyOrder_HasOneDividerAboveEachSectionAndNoneAfterTheLast(string order)
        {
            var requested = order.Split(',');
            var pdf = Render(TemplateTypes.Simplified, FullResume(requested));

            AssertOneDividerAboveEachSection(
                pdf,
                requested.Select(key => SimplifiedTitles[key].FirstWord).ToList()
            );
        }

        /// <summary>
        /// There is one divider under the header, then one directly above each section, so the
        /// section at position i has i + 2 dividers above it and the page has sections + 1 in all.
        /// </summary>
        private static void AssertOneDividerAboveEachSection(byte[] pdf, List<string> firstWords)
        {
            var dividers = PdfText.GetDividerYs(pdf);
            Assert.Equal(firstWords.Count + 1, dividers.Count);

            for (var i = 0; i < firstWords.Count; i++)
            {
                var y = PdfText.GetWordY(pdf, firstWords[i]);
                Assert.Equal(i + 2, dividers.Count(d => d > y));
            }
        }

        private static byte[] Render(string template, ResumeDTO resume)
        {
            IDocument document = template == TemplateTypes.Classic
                ? new ResumePdfGenerator(resume)
                : new SimplifiedResumePdfGenerator(resume);
            return document.GeneratePdf();
        }

        private static Dictionary<string, (string Text, string FirstWord)> Titles(string template)
        {
            return template == TemplateTypes.Classic ? ClassicTitles : SimplifiedTitles;
        }

        private static List<string> HeadingsInOrder(byte[] pdf, string template)
        {
            return PdfText.HeadingsInOrder(pdf, Titles(template).Values.Select(t => t.Text));
        }

        private static ResumeDTO FullResume(IEnumerable<string?>? order)
        {
            return new ResumeDTO
            {
                Name = "Jane Tester",
                Title = "Engineer",
                Email = "jane@test.local",
                PhoneNumber = "0820000001",
                Summary = "A short summary.",
                SectionOrder = order?.ToList()!,
                Skills = [new SkillsItem { Skill = "CSharp", SkillLevel = "Expert" }],
                Experience =
                [
                    new ExperienceItem
                    {
                        Company = "Acme",
                        JobTitle = "Developer",
                        StartDate = "January 2020",
                        EndDate = "May 2022",
                        Responsibilities = ["Built things"],
                    },
                ],
                Education =
                [
                    new EducationItem { Institution = "Uni", Qualification = "BSc", Major = "CS" },
                ],
                Certification = [new CertificationItem { Name = "AZ-900", Organisation = "Microsoft" }],
            };
        }

        private static IEnumerable<List<string>> Permutations(List<string> items)
        {
            if (items.Count <= 1)
            {
                yield return [.. items];
                yield break;
            }

            for (var i = 0; i < items.Count; i++)
            {
                var rest = items.Where((_, index) => index != i).ToList();
                foreach (var tail in Permutations(rest))
                {
                    yield return [items[i], .. tail];
                }
            }
        }
    }
}
