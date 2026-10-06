using Portfolio.Core.Exceptions;

namespace Portfolio.Infrastructure.Repositories
{
    /// <summary>
    /// Parses dates sent by clients. Accepts the same formats as DateOnly.Parse, but a bad value
    /// becomes a ValidationException (400) instead of a FormatException (500).
    /// </summary>
    internal static class DateInput
    {
        public static DateOnly Parse(string? value, string field)
        {
            if (DateOnly.TryParse(value, out var date))
                return date;

            throw new ValidationException($"{field} is not a valid date.");
        }
    }
}
