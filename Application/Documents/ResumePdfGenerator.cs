using Portfolio.Core.Constants;
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

        protected override IReadOnlyList<string> DefaultSectionOrder { get; } =
            [
                ResumeSections.Experience,
                ResumeSections.Skills,
                ResumeSections.Education,
                ResumeSections.Certifications,
            ];

        // Classic puts a gap after the Summary and after every section slot except the last, drawn
        // whether or not the section has data, so the default order keeps its original spacing.
        protected override void AfterSection(ColumnDescriptor column, bool populated, bool isLast)
        {
            if (!isLast)
                column.Item().Padding(5);
        }

        protected override void RenderSummary(ColumnDescriptor column)
        {
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
        }

        protected override void RenderExperience(ColumnDescriptor column)
        {
            List<ExperienceItem> experience = Model.Experience!;

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

            foreach (var (exp, index) in experience.Select((item, i) => (item, i)))
            {
                var jobCompanyName = exp.Company ?? string.Empty;
                var jobTitle = exp.JobTitle ?? string.Empty;
                var jobDates = string.Join(
                    " - ",
                    new[] { exp.StartDate, exp.EndDate }.Where(s => !string.IsNullOrWhiteSpace(s))
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
                            table.Cell().Row(2).Column(1).Text(jobCompanyName).Bold().AlignLeft();
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

                RenderItemGap(column, index, experience.Count);
            }
        }

        protected override void RenderSkills(ColumnDescriptor column)
        {
            List<SkillsItem> skills = Model.Skills!;

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
                                    ? $"{skillName} — {skillLevel}"
                                    : skillName
                            );
                    });
            }
        }

        protected override void RenderEducation(ColumnDescriptor column)
        {
            List<EducationItem> education = Model.Education!;

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

            foreach (var (ed, index) in education.Select((item, i) => (item, i)))
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
                RenderItemGap(column, index, education.Count);
            }
        }

        protected override void RenderCertifications(ColumnDescriptor column)
        {
            List<CertificationItem> certification = Model.Certification!;

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
    }
}
