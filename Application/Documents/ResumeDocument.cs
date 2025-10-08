using Portfolio.Core.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Portfolio.Application.Documents
{
    public class ResumeDocument : IDocument
    {
        private readonly ResumeDto _m;
        public ResumeDocument(ResumeDto model) => _m = model ?? new();

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

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
                    // Education
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("EDUCATION").Bold().FontSize(16);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    if (_m.Education?.Any() == true)
                    {
                        var ed = _m.Education.First();
                        var institutionName = ed.Institution ?? "";
                        var qualification = ed.Degree ?? "";
                        var datesStudied = string.Join(" - ", new[] { ed.Start, ed.End }.Where(s => !string.IsNullOrWhiteSpace(s)));
                        var eduLocation = ""; // Not present in DTO
                        var eduDescription = ed.Additional ?? "";
                        var coursework = new string[0]; // Not present in DTO

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Cell().Row(1).Column(1).Text(institutionName).Bold().AlignLeft();
                            table.Cell().Row(1).Column(2).Text(datesStudied).Bold().AlignRight();
                            table.Cell().Row(2).Column(1).Text(qualification).AlignLeft();
                            table.Cell().Row(2).Column(2).Text(eduLocation).AlignRight();
                        });
                        if (!string.IsNullOrWhiteSpace(eduDescription))
                            column.Item().Text($"- {eduDescription}");
                        if (coursework.Any())
                        {
                            string courseworkText = string.Join(", ", coursework);
                            column.Item().Text($"- Coursework: {courseworkText}");
                        }
                    }

                    column.Item().Padding(5);

                    // Experience
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("EXPERIENCE").Bold().FontSize(16);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    if (_m.Experience?.Any() == true)
                    {
                        var exp = _m.Experience.First();
                        var jobCompanyName = exp.Company ?? "";
                        var jobTitle = exp.Role ?? "";
                        var jobDates = string.Join(" - ", new[] { exp.Start, exp.End }.Where(s => !string.IsNullOrWhiteSpace(s)));
                        var jobLocation = exp.Location ?? "";
                        var jobResponsibilities = exp.Bullets ?? new List<string>();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Cell().Row(1).Column(1).Text(jobCompanyName).Bold().AlignLeft();
                            table.Cell().Row(1).Column(2).Text(jobDates).Bold().AlignRight();
                            table.Cell().Row(2).Column(1).Text(jobTitle).AlignLeft();
                            table.Cell().Row(2).Column(2).Text(jobLocation).AlignRight();
                        });

                        foreach (string res in jobResponsibilities)
                        {
                            column.Item().Row(row =>
                            {
                                row.AutoItem().Text("-");
                                row.ConstantItem(5);
                                row.RelativeItem().Text(res);
                            });
                        }
                    }

                    column.Item().Padding(3);

                    // Skills
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("SKILLS").Bold().FontSize(16);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    if (_m.Skills?.Any() == true)
                    {
                        string skillsText = string.Join(", ", _m.Skills);
                        column.Item().Text(text =>
                        {
                            text.Span("- ");
                            text.Span("Skills").Bold();
                            text.Span(": " + skillsText);
                        });
                    }

                    column.Item().Padding(5);

                    // Projects (not present in DTO, so placeholder)
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("PROJECTS").Bold().FontSize(16);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    column.Item().Text("No projects listed.");

                    column.Item().Padding(5);

                    // Awards (not present in DTO, so placeholder)
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("AWARDS").Bold().FontSize(16);
                        row.AutoItem().PaddingHorizontal(10);
                        row.RelativeItem().AlignMiddle().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });
                    column.Item().Text("No awards listed.");
                });
            });
        }
    }
}
