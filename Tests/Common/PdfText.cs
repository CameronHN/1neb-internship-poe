using UglyToad.PdfPig;

namespace Portfolio.Tests.Common
{
    public static class PdfText
    {
        /// <summary>
        /// Returns all text in the PDF with every whitespace character removed.
        /// Spaces are dropped because PDF text extraction does not always keep them,
        /// so compare against values that contain no spaces.
        /// </summary>
        public static string ExtractWithoutWhitespace(byte[] pdf)
        {
            using var document = PdfDocument.Open(pdf);
            var text = string.Concat(document.GetPages().Select(page => page.Text));
            return new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        /// <summary>
        /// The given headings, in the order they appear on the page (top to bottom, across
        /// pages). A heading that is not in the PDF is left out. Headings must be unique in the
        /// text, so pass the upper-case section titles.
        /// </summary>
        public static List<string> HeadingsInOrder(byte[] pdf, IEnumerable<string> headings)
        {
            var text = ExtractWithoutWhitespace(pdf);
            return headings
                .Select(heading => (heading, index: text.IndexOf(heading, StringComparison.Ordinal)))
                .Where(found => found.index >= 0)
                .OrderBy(found => found.index)
                .Select(found => found.heading)
                .ToList();
        }

        /// <summary>
        /// The vertical position (PDF units, larger is higher) of every horizontal rule the
        /// templates draw with LineHorizontal, highest first. Page backgrounds are not counted.
        /// </summary>
        public static List<double> GetDividerYs(byte[] pdf)
        {
            using var document = PdfDocument.Open(pdf);
            return document
                .GetPages()
                .SelectMany(page => page.Paths)
                .Where(path => path.IsStroked && !path.IsFilled)
                .Select(path => path.GetBoundingRectangle())
                .Where(rectangle => rectangle is { Height: 0 })
                .Select(rectangle => (double)rectangle!.Value.Bottom)
                .OrderByDescending(y => y)
                .ToList();
        }

        /// <summary>
        /// The vertical position of the first word with this exact text (single page PDFs only).
        /// </summary>
        public static double GetWordY(byte[] pdf, string word)
        {
            using var document = PdfDocument.Open(pdf);
            return document
                .GetPages()
                .SelectMany(page => page.GetWords())
                .First(w => w.Text == word)
                .BoundingBox.Bottom;
        }

        /// <summary>
        /// The target of every clickable link (URI link annotation) in the PDF.
        /// </summary>
        public static List<string> GetLinkUris(byte[] pdf)
        {
            using var document = PdfDocument.Open(pdf);
            return document
                .GetPages()
                .SelectMany(page => page.GetAnnotations())
                .Select(annotation => annotation.Action)
                .OfType<UglyToad.PdfPig.Actions.UriAction>()
                .Select(action => action.Uri)
                .ToList();
        }
    }
}
