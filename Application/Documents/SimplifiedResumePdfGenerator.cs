using Portfolio.Core.Constants;
using Portfolio.Core.DTOs.Resume;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Portfolio.Application.Documents
{
    public class SimplifiedResumePdfGenerator : BaseResumePdfGenerator
    {
        private readonly float sectionTitleFontSize = 12;

        public SimplifiedResumePdfGenerator(ResumeDTO model)
            : base(model) { }

        protected override IReadOnlyList<string> DefaultSectionOrder { get; } =
            [
                ResumeSections.Skills,
                ResumeSections.Experience,
                ResumeSections.Education,
                ResumeSections.Certifications,
            ];

        // Single column, left aligned, single list of skills.
        // The header block ends with a divider and the Summary has none of its own, so the
        // Summary always follows the header directly. Each movable section that has data is
        // drawn between its own divider and a gap, so no order gives a doubled or leading divider.
        protected override void RenderSummary(ColumnDescriptor column)
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            column.Item().Padding(5);

            var summary = Model.Summary;
            if (!string.IsNullOrWhiteSpace(summary))
            {
                column.Item().Padding(5);

                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text("SUMMARY").Bold().FontSize(sectionTitleFontSize);
                        row.AutoItem().Padding(3);
                    });

                column.Item().Row(row => row.RelativeItem().Text(summary));
                column.Item().Padding(5);
            }
        }

        protected override void BeforeSection(ColumnDescriptor column)
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            column.Item().Padding(5);
        }

        protected override void AfterSection(ColumnDescriptor column, bool populated, bool isLast)
        {
            if (populated)
                column.Item().Padding(5);
        }

        protected override void RenderSkills(ColumnDescriptor column)
        {
            var skills = Model.Skills!;

            column
                .Item()
                .Row(row =>
                {
                    row.AutoItem().Text("SKILLS").Bold().FontSize(sectionTitleFontSize);
                    row.AutoItem().Padding(3);
                });

            var skillNames = string.Join(
                ", ",
                skills.Select(s => s?.Skill?.Trim()).Where(sn => !string.IsNullOrWhiteSpace(sn))
            );

            column.Item().Row(row => row.RelativeItem().Text(skillNames));
        }

        protected override void RenderExperience(ColumnDescriptor column)
        {
            var experience = Model.Experience!;

            column
                .Item()
                .Row(row =>
                {
                    row.AutoItem()
                        .Text("PROFESSIONAL EXPERIENCE")
                        .Bold()
                        .FontSize(sectionTitleFontSize);
                    row.AutoItem().Padding(3);
                });

            foreach (var (exp, index) in experience.Select((item, i) => (item, i)))
            {
                var jobCompanyName = exp.Company ?? string.Empty;
                var jobTitle = exp.JobTitle ?? string.Empty;
                var jobDates = JoinDates(exp.StartDate, exp.EndDate);
                var jobResponsibilities = exp.Responsibilities ?? new List<string>();

                column.Item().Row(row => row.AutoItem().Text(jobTitle).Bold().AlignLeft());

                if (
                    !string.IsNullOrWhiteSpace(jobCompanyName)
                    || !string.IsNullOrWhiteSpace(jobDates)
                )
                {
                    column
                        .Item()
                        .Row(row =>
                        {
                            row.AutoItem()
                                .Text(
                                    $"{jobCompanyName}{(string.IsNullOrWhiteSpace(jobCompanyName) || string.IsNullOrWhiteSpace(jobDates) ? string.Empty : " | ")}{jobDates}"
                                );
                        });
                }

                foreach (string res in jobResponsibilities)
                {
                    column
                        .Item()
                        .Row(row =>
                        {
                            row.AutoItem().Text(bulletpoint);
                            row.ConstantItem(5);
                            row.RelativeItem().Text(res);
                        });
                }

                RenderItemGap(column, index, experience.Count);
            }
        }

        protected override void RenderEducation(ColumnDescriptor column)
        {
            var education = Model.Education!;

            column
                .Item()
                .Row(row =>
                {
                    row.AutoItem().Text("EDUCATION").Bold().FontSize(sectionTitleFontSize);
                    row.AutoItem().Padding(3);
                });

            foreach (var (ed, index) in education.Select((item, i) => (item, i)))
            {
                var institutionName = ed.Institution ?? string.Empty;
                var qualification = ed.Qualification ?? string.Empty;
                var datesStudied = JoinDates(ed.StartDate, ed.EndDate);
                var major = ed.Major ?? string.Empty;

                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem()
                            .Text(text =>
                            {
                                text.Span(qualification).Bold();
                                if (!string.IsNullOrWhiteSpace(major))
                                    text.Span($", majored in {major}");
                            });
                    });

                column
                    .Item()
                    .Row(row =>
                        row.AutoItem()
                            .Text(
                                $"{institutionName}{(string.IsNullOrWhiteSpace(institutionName) || string.IsNullOrWhiteSpace(datesStudied) ? string.Empty : " | ")}{datesStudied}"
                            )
                    );
                RenderItemGap(column, index, education.Count);
            }
        }

        protected override void RenderCertifications(ColumnDescriptor column)
        {
            var certification = Model.Certification!;

            column
                .Item()
                .Row(row =>
                {
                    row.AutoItem().Text("CERTIFICATIONS").Bold().FontSize(sectionTitleFontSize);
                    row.AutoItem().Padding(3);
                });

            foreach (var ce in certification)
            {
                var cert = ce.Name ?? string.Empty;
                var hasSafeLink = SafeLink.TryGetSafeUri(ce.CredentialUrl, out var certLink);
                var org = ce.Organisation ?? string.Empty;

                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text(bulletpoint);
                        row.ConstantItem(5);

                        row.RelativeItem()
                            .Text(text =>
                            {
                                text.Span(cert).Bold();
                                if (hasSafeLink)
                                {
                                    text.Span(" (").Bold();
                                    text.Hyperlink("Link", certLink)
                                        .FontColor(Colors.Blue.Medium)
                                        .Bold();
                                    text.Span(")").Bold();
                                }

                                if (!string.IsNullOrWhiteSpace(org))
                                    text.Span($", {org}");
                            });
                    });
            }
        }

        private static string JoinDates(string? startDate, string? endDate)
        {
            return string.Join(
                " - ",
                new[] { startDate, endDate }.Where(s => !string.IsNullOrWhiteSpace(s))
            );
        }
    }
}
