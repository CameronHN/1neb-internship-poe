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

        // Single column, left aligned, single list of skills
        protected override void ComposeContent(ColumnDescriptor column)
        {
            // Render header from base (includes name, title, email/phone and social links)
            RenderHeader(column);

            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            column.Item().Padding(5);

            // SUMMARY
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

            // SKILLS
            var skills = Model.Skills;
            if (skills?.Count > 0)
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                column.Item().Padding(5);

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
                column.Item().Padding(5);
            }

            // EXPERIENCE
            var experience = Model.Experience;
            if (experience?.Count > 0)
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                column.Item().Padding(5);

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

                foreach (var exp in experience)
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

                    column.Item().Padding(3);
                }

                column.Item().Padding(5);
            }

            // EDUCATION
            var education = Model.Education;
            if (education?.Count > 0)
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                column.Item().Padding(5);

                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text("EDUCATION").Bold().FontSize(sectionTitleFontSize);
                        row.AutoItem().Padding(3);
                    });

                foreach (var ed in education)
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
                    column.Item().Padding(3);
                }

                column.Item().Padding(5);
            }

            // CERTIFICATIONS
            var certification = Model.Certification;
            if (certification?.Count > 0)
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                column.Item().Padding(5);

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
                    var certLink = ce.CredentialUrl ?? string.Empty;
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
                                    if (!string.IsNullOrWhiteSpace(certLink))
                                    {
                                        text.Span(" (").Bold();
                                        // TODO: Use safe hyperlinking. Base on RenderSocialLinksInline
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
