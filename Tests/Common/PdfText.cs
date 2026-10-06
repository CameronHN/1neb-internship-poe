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
