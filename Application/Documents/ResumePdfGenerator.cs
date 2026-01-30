using Portfolio.Core.DTOs.Resume;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Portfolio.Application.Documents
{
    public class ResumePdfGenerator : BaseResumePdfGenerator
    {
        public ResumePdfGenerator(ResumeDTO model)
            : base(model) { }

        protected override void RenderHeader(ColumnDescriptor column)
        {
            column
                .Item()
                .Text(Model.Name ?? string.Empty)
                .Bold()
                .FontSize(30)
                .FontColor(Colors.Black)
                .AlignCenter();

            var title = Model.Title;
            if (!string.IsNullOrWhiteSpace(title))
            {
                column.Item().Text(title).Bold().FontSize(20).FontColor(Colors.Black).AlignCenter();

                column.Item().Padding(3);
            }

            column
                .Item()
                .Row(row =>
                {
                    row.RelativeItem()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span(
                                $"{Model.Email ?? string.Empty} | {Model.PhoneNumber ?? string.Empty}"
                            );

                            RenderSocialLinksInline(text);
                        });
                });

            column.Item().Padding(5);
        }

        protected override void ComposeContent(ColumnDescriptor column)
        {
            // Render header (uses the overridden centered header)
            RenderHeader(column);

            // Summary
            string? summary = Model.Summary;
            if (!string.IsNullOrEmpty(summary))
            {
                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text("SUMMARY").Bold().FontSize(14);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem()
                            .AlignMiddle()
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                        row.AutoItem().Padding(3);
                    });

                column.Item().Row(row => row.RelativeItem().Text(summary));
            }

            column.Item().Padding(5);

            // Experience
            List<ExperienceItem>? experience = Model.Experience;
            if (experience?.Count > 0)
            {
                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text("EXPERIENCE").Bold().FontSize(14);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem()
                            .AlignMiddle()
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                        row.AutoItem().Padding(3);
                    });

                foreach (var exp in experience)
                {
                    var jobCompanyName = exp.Company ?? string.Empty;
                    var jobTitle = exp.JobTitle ?? string.Empty;
                    var jobDates = string.Join(
                        " - ",
                        new[] { exp.StartDate, exp.EndDate }.Where(s =>
                            !string.IsNullOrWhiteSpace(s)
                        )
                    );
                    var jobResponsibilities = exp.Responsibilities ?? new List<string>();

                    column
                        .Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Cell().Row(1).Column(1).Text(jobTitle).Bold().AlignLeft();
                            table.Cell().Row(1).Column(2).Text(jobDates).Bold().AlignRight();
                            if (!string.IsNullOrWhiteSpace(jobCompanyName))
                            {
                                table
                                    .Cell()
                                    .Row(2)
                                    .Column(1)
                                    .Text(jobCompanyName)
                                    .Bold()
                                    .AlignLeft();
                            }
                        });

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
            }

            column.Item().Padding(5);

            // Skills
            List<SkillsItem>? skills = Model.Skills;
            if (skills?.Count > 0)
            {
                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text("SKILLS").Bold().FontSize(14);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem()
                            .AlignMiddle()
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                        row.AutoItem().Padding(3);
                    });

                foreach (var skill in skills)
                {
                    var skillName = skill.Skill ?? string.Empty;
                    var skillLevel = skill.SkillLevel ?? string.Empty;

                    column
                        .Item()
                        .Row(row =>
                        {
                            row.AutoItem().Text(bulletpoint);
                            row.ConstantItem(5);
                            row.RelativeItem()
                                .Text(
                                    !string.IsNullOrWhiteSpace(skillLevel)
                                        ? $"{skillName} \u2014 {skillLevel}"
                                        : skillName
                                );
                        });
                }
            }

            column.Item().Padding(5);

            // Education
            List<EducationItem>? education = Model.Education;
            if (education?.Count > 0)
            {
                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text("EDUCATION").Bold().FontSize(14);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem()
                            .AlignMiddle()
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                        row.AutoItem().Padding(3);
                    });

                foreach (var ed in education)
                {
                    var institutionName = ed.Institution ?? string.Empty;
                    var qualification = ed.Qualification ?? string.Empty;
                    var datesStudied = string.Join(
                        " - ",
                        new[] { ed.StartDate, ed.EndDate }.Where(s => !string.IsNullOrWhiteSpace(s))
                    );
                    var major = ed.Major ?? string.Empty;

                    column
                        .Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table
                                .Cell()
                                .Row(1)
                                .Column(1)
                                .Text(
                                    qualification
                                        + (
                                            !string.IsNullOrWhiteSpace(major)
                                                ? $", majored in {major}"
                                                : string.Empty
                                        )
                                )
                                .Bold()
                                .AlignLeft();
                            table.Cell().Row(1).Column(2).Text(datesStudied).Bold().AlignRight();
                            table.Cell().Row(2).Column(1).Text(institutionName).AlignLeft();
                        });
                    column.Item().Padding(3);
                }
            }

            column.Item().Padding(5);

            // Certifications
            List<CertificationItem>? certification = Model.Certification;
            if (certification?.Count > 0)
            {
                column
                    .Item()
                    .Row(row =>
                    {
                        row.AutoItem().Text("CERTIFICATIONS").Bold().FontSize(14);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem()
                            .AlignMiddle()
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
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
    }
}
