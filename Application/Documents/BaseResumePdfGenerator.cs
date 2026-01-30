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

        // Shared social rendering with filtering (prevents QuestPDF Url null/empty errors)
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
                text.Hyperlink(s.LinkType!, s.Link!).FontColor(Colors.Blue.Medium);

                if (i < socials.Count - 1)
                    text.Span(" | ");
            }
        }

        // Derived classes implement / override this to render body content
        protected abstract void ComposeContent(ColumnDescriptor column);
    }
}
