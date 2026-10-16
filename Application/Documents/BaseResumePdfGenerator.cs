using Portfolio.Core.Constants;
using Portfolio.Core.DTOs.Resume;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Portfolio.Application.Documents
{
    public abstract class BaseResumePdfGenerator : IDocument
    {
        protected readonly ResumeDTO Model;
        protected readonly string bulletpoint = "•";

        protected BaseResumePdfGenerator(ResumeDTO model)
        {
            Model = model ?? new ResumeDTO();
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));
                page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial));
                page.DefaultTextStyle(x => x.FontColor(Colors.Black));

                page.Content()
                    .Column(column =>
                    {
                        ComposeContent(column);
                    });
            });
        }

        // Default header implementation (can be overridden)
        protected virtual void RenderHeader(ColumnDescriptor column)
        {
            column
                .Item()
                .Text(Model.Name ?? string.Empty)
                .Bold()
                .FontSize(30)
                .FontColor(Colors.Black)
                .AlignLeft();

            var title = Model.Title;
            if (!string.IsNullOrWhiteSpace(title))
            {
                column.Item().Text(title).Bold().FontSize(20).FontColor(Colors.Black).AlignLeft();

                column.Item().Padding(3);
            }

            column
                .Item()
                .Row(row =>
                {
                    row.RelativeItem()
                        .AlignLeft()
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

        // Shared social rendering with filtering (prevents QuestPDF Url null/empty errors).
        // Only http(s) links become clickable; anything else shows its label as plain text.
        protected void RenderSocialLinksInline(TextDescriptor text)
        {
            var socials = Model
                .ProfessionalLinks?.Where(s =>
                    !string.IsNullOrWhiteSpace(s?.Link) && !string.IsNullOrWhiteSpace(s?.LinkType)
                )
                .ToList();

            if (socials == null || socials.Count == 0)
                return;

            // leading separator
            text.Span(" | ");

            for (int i = 0; i < socials.Count; i++)
            {
                var s = socials[i]!;
                if (SafeLink.TryGetSafeUri(s.Link, out var uri))
                    text.Hyperlink(s.LinkType!, uri).FontColor(Colors.Blue.Medium);
                else
                    text.Span(s.LinkType!);

                if (i < socials.Count - 1)
                    text.Span(" | ");
            }
        }

        // The header and the Summary are fixed at the top. Only the ResumeSection values move, in
        // the order from ResumeDTO.SectionOrder (or the template's default).
        private void ComposeContent(ColumnDescriptor column)
        {
            RenderHeader(column);
            RenderSummary(column);

            var order = ResumeSections.Resolve(Model.SectionOrder, DefaultSectionOrder);
            for (var i = 0; i < order.Count; i++)
            {
                var populated = HasData(order[i]);
                if (populated)
                {
                    BeforeSection(column);
                    RenderSection(column, order[i]);
                }

                AfterSection(column, populated, isLast: i == order.Count - 1);
            }
        }

        // The movable sections in the order this template draws them when no order is requested.
        protected abstract IReadOnlyList<ResumeSection> DefaultSectionOrder { get; }

        // Draws the Summary and any spacing or divider around it. Skipped content stays skipped.
        protected abstract void RenderSummary(ColumnDescriptor column);

        // Divider/spacing drawn before a section that has data.
        protected virtual void BeforeSection(ColumnDescriptor column) { }

        // Spacing drawn once per section slot, after it. populated is false when the section was skipped.
        protected virtual void AfterSection(ColumnDescriptor column, bool populated, bool isLast) { }

        // Small gap between the items of a section (not after the last one, where the section
        // gap already applies).
        protected static void RenderItemGap(ColumnDescriptor column, int index, int count)
        {
            if (index < count - 1)
                column.Item().Padding(3);
        }

        protected abstract void RenderSkills(ColumnDescriptor column);

        protected abstract void RenderExperience(ColumnDescriptor column);

        protected abstract void RenderEducation(ColumnDescriptor column);

        protected abstract void RenderCertifications(ColumnDescriptor column);

        // A new ResumeSection member must be added to both switches below. The default arm throws
        // (instead of skipping the section) so a missed case fails loudly in every test that
        // renders a resume, not silently in production.
        private bool HasData(ResumeSection section)
        {
            return section switch
            {
                ResumeSection.Skills => Model.Skills?.Count > 0,
                ResumeSection.Experience => Model.Experience?.Count > 0,
                ResumeSection.Education => Model.Education?.Count > 0,
                ResumeSection.Certifications => Model.Certification?.Count > 0,
                _ => throw UnhandledSection(section),
            };
        }

        private void RenderSection(ColumnDescriptor column, ResumeSection section)
        {
            switch (section)
            {
                case ResumeSection.Skills:
                    RenderSkills(column);
                    break;
                case ResumeSection.Experience:
                    RenderExperience(column);
                    break;
                case ResumeSection.Education:
                    RenderEducation(column);
                    break;
                case ResumeSection.Certifications:
                    RenderCertifications(column);
                    break;
                default:
                    throw UnhandledSection(section);
            }
        }

        private static ArgumentOutOfRangeException UnhandledSection(ResumeSection section)
        {
            return new ArgumentOutOfRangeException(
                nameof(section),
                section,
                "This resume section is not handled by the PDF generator."
            );
        }
    }
}
