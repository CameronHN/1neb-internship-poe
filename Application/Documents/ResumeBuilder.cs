using Portfolio.Core.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Portfolio.Application.Documents
{
    public class ResumeBuilder : IDocument
    {
        private readonly ResumeDto _m;
        public ResumeBuilder(ResumeDto model) => _m = model ?? new();

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

                page.Header().Column(column =>
                {
                    column.Item().Text(_m.Name ?? "Your Name").Bold().FontSize(30).FontColor(Colors.Blue.Medium).AlignCenter();

                    var contactParts = new[]
                    {
                        _m.Contact?.Website,
                        _m.Contact?.Phone,
                        _m.Contact?.Email,
                        _m.Contact?.LinkedIn,
                        _m.Contact?.Github
                    }.Where(s => !string.IsNullOrWhiteSpace(s));
                    column.Item().Text(string.Join(" | ", contactParts)).AlignCenter();
                    column.Item().Padding(5);
                });

                page.Content().Column(column =>
                {
                    // Experience
                    if (_m.Experience?.Any() == true)
                    {
                        column.Item().Row(row =>
                        {
                            row.AutoItem().Text("EXPERIENCE").Bold().FontSize(14);
                            row.AutoItem().PaddingHorizontal(10);
                            row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            row.AutoItem().Padding(3);
                        });

                        foreach (var exp in _m.Experience)
                        {
                            var jobCompanyName = exp.Company ?? "Unknown";
                            var jobTitle = exp.Role ?? "Unknown";
                            var jobDates = string.Join(" - ", new[] { exp.Start, exp.End }.Where(s => !string.IsNullOrWhiteSpace(s)));
                            var jobResponsibilities = exp.Responsibilities ?? new List<string>();

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Cell().Row(1).Column(1).Text(jobTitle).Bold().AlignLeft();
                                table.Cell().Row(1).Column(2).Text(jobDates).Bold().AlignRight();
                                table.Cell().Row(2).Column(1).Text(jobCompanyName).Bold().AlignLeft();
                            });

                            foreach (string res in jobResponsibilities)
                            {
                                column.Item().Row(row =>
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
                    if (_m.Skills?.Any() == true)
                    {
                        column.Item().Row(row =>
                        {
                            row.AutoItem().Text("SKILLS").Bold().FontSize(14);
                            row.AutoItem().PaddingHorizontal(10);
                            row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            row.AutoItem().Padding(3);
                        });

                        foreach (var skill in _m.Skills)
                        {
                            var skillName = skill.Skill ?? "";
                            var skillLevel = skill.SkillLevel ?? "";

                            column.Item().Row(row =>
                            {
                                row.AutoItem().Text(bulletpoint);
                                row.ConstantItem(5);
                                row.RelativeItem().Text($"{skillName} \u2014 {skillLevel}");
                            });
                        }
                    }

                    column.Item().Padding(5);

                    // Education
                    if (_m.Education?.Any() == true)
                    {
                        column.Item().Row(row =>
                        {
                            row.AutoItem().Text("EDUCATION").Bold().FontSize(14);
                            row.AutoItem().PaddingHorizontal(10);
                            row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            row.AutoItem().Padding(3);
                        });

                        foreach (var ed in _m.Education)
                        {
                            var institutionName = ed.Institution ?? "";
                            var qualification = ed.Degree ?? "";
                            var datesStudied = string.Join(" - ", new[] { ed.Start, ed.End }.Where(s => !string.IsNullOrWhiteSpace(s)));
                            var major = ed.Major ?? "";

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Cell().Row(1).Column(1).Text(qualification + (!string.IsNullOrWhiteSpace(major) ? ", " + major : "")).Bold().AlignLeft();
                                table.Cell().Row(1).Column(2).Text(datesStudied).Bold().AlignRight();
                                table.Cell().Row(2).Column(1).Text(institutionName).AlignLeft();
                            });
                            column.Item().Padding(3);
                        }
                    }

                    column.Item().Padding(5);

                    // Certifications
                    if (_m.Certification?.Any() == true)
                    {
                        column.Item().Row(row =>
                        {
                            row.AutoItem().Text("CERTIFICATIONS").Bold().FontSize(14);
                            row.AutoItem().PaddingHorizontal(10);
                            row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            row.AutoItem().Padding(3);
                        });

                        foreach (var ce in _m.Certification)
                        {
                            var cert = ce.Name ?? "Unknown";
                            var certLink = ce.CredentialUrl ?? "";
                            var org = ce.Organisation ?? "";

                            column.Item().Row(row =>
                            {
                                row.AutoItem().Text(bulletpoint);
                                row.ConstantItem(5);

                                row.RelativeItem().Text(text =>
                                {
                                    text.Span(cert).Bold();
                                    if (!string.IsNullOrWhiteSpace(certLink))
                                    {
                                        text.Span(" (").Bold();
                                        text.Hyperlink("Link", certLink).FontColor(Colors.Blue.Medium).Bold();
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
