using Portfolio.Core.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Portfolio.Application.Documents
{
    public class ResumeBuilder : IDocument
    {
        private readonly ResumeDto _m;

        public ResumeBuilder(ResumeDto model)
        {
            _m = model ?? new ResumeDto { Email = string.Empty, PhoneNumber = string.Empty };
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        private readonly string bulletpoint = "•\t\t\t\t\t";

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));
                page.Margin(30);

                page.Header()
                    .Column(column =>
                    {
                        column
                            .Item()
                            .Text(_m.Name)
                            .Bold()
                            .FontSize(30)
                            .FontColor(Colors.Black)
                            .AlignCenter();

                        // Summary
                        string? title = _m.Title;
                        if (title?.Any() == true)
                        {
                            column
                                .Item()
                                .Text(_m.Title)
                                .Bold()
                                .FontSize(20)
                                .FontColor(Colors.Black)
                                .AlignCenter();

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
                                        text.Span(_m.Email + " | " + _m.PhoneNumber);

                                        // Socials
                                        if (_m.Socials?.Any() == true)
                                        {
                                            foreach (var s in _m.Socials)
                                            {
                                                var url = s?.SocialMediaUrl;
                                                if (!string.IsNullOrWhiteSpace(url))
                                                {
                                                    text.Span(" | ");
                                                    text.Hyperlink(url, url);
                                                }
                                            }
                                        }
                                    });
                            });
                        column.Item().Padding(5);
                    });

                page.Content()
                    .Column(column =>
                    {
                        // Summary
                        string? summary = _m.Summary;
                        if (summary?.Any() == true)
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

                            var summaryText = summary;

                            column.Item().Row(row => row.RelativeItem().Text(summaryText));
                        }

                        column.Item().Padding(5);

                        // Experience
                        List<ExperienceItem>? experience = _m.Experience;
                        if (experience?.Any() == true)
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
                                var jobCompanyName = exp.Company ?? "Unknown";
                                var jobTitle = exp.JobTitle ?? "Unknown";
                                var jobDates = string.Join(
                                    " - ",
                                    new[] { exp.StartDate, exp.EndDate }.Where(s =>
                                        !string.IsNullOrWhiteSpace(s)
                                    )
                                );
                                var jobResponsibilities =
                                    exp.Responsibilities ?? new List<string>();

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
                                            .Text(jobTitle)
                                            .Bold()
                                            .AlignLeft();
                                        table
                                            .Cell()
                                            .Row(1)
                                            .Column(2)
                                            .Text(jobDates)
                                            .Bold()
                                            .AlignRight();
                                        table
                                            .Cell()
                                            .Row(2)
                                            .Column(1)
                                            .Text(jobCompanyName)
                                            .Bold()
                                            .AlignLeft();
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
                        List<SkillsItem>? skills = _m.Skills;
                        if (skills?.Any() == true)
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
                                var skillName = skill.Skill ?? "";
                                var skillLevel = skill.SkillLevel ?? "";

                                column
                                    .Item()
                                    .Row(row =>
                                    {
                                        row.AutoItem().Text(bulletpoint);
                                        row.ConstantItem(5);
                                        row.RelativeItem().Text($"{skillName} \u2014 {skillLevel}");
                                    });
                            }
                        }

                        column.Item().Padding(5);

                        // Education
                        List<EducationItem>? education = _m.Education;
                        if (education?.Any() == true)
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
                                var institutionName = ed.Institution ?? "";
                                var qualification = ed.Qualification ?? "";
                                var datesStudied = string.Join(
                                    " - ",
                                    new[] { ed.StartDate, ed.EndDate }.Where(s =>
                                        !string.IsNullOrWhiteSpace(s)
                                    )
                                );
                                var major = ed.Major ?? "";

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
                                                            ? ", " + major
                                                            : ""
                                                    )
                                            )
                                            .Bold()
                                            .AlignLeft();
                                        table
                                            .Cell()
                                            .Row(1)
                                            .Column(2)
                                            .Text(datesStudied)
                                            .Bold()
                                            .AlignRight();
                                        table
                                            .Cell()
                                            .Row(2)
                                            .Column(1)
                                            .Text(institutionName)
                                            .AlignLeft();
                                    });
                                column.Item().Padding(3);
                            }
                        }

                        column.Item().Padding(5);

                        // Certifications
                        List<CertificationItem>? certification = _m.Certification;
                        if (certification?.Any() == true)
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
                                var cert = ce.Name ?? "Unknown";
                                var certLink = ce.CredentialUrl ?? "";
                                var org = ce.Organisation ?? "";

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
                    });
            });
        }
    }
}
