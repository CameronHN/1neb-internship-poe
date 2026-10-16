using System.ComponentModel.DataAnnotations;
using Portfolio.Core.Constants;

namespace Portfolio.Core.DTOs.Resume
{
    /// <summary>
    /// Rejects a <c>SectionOrder</c> with an unknown key or a duplicate. Matching ignores case and
    /// surrounding spaces. Null and empty are valid (they mean "use the template's default").
    /// Only runs on model-bound requests; saved snapshots are read leniently by
    /// <see cref="ResumeSections.Resolve"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ValidSectionOrderAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IEnumerable<string?> keys)
                return ValidationResult.Success;

            var seen = new HashSet<string>();
            foreach (var key in keys)
            {
                var normalised = key?.Trim().ToLowerInvariant();
                if (normalised == null || !ResumeSections.IsKnown(normalised))
                {
                    return new ValidationResult(
                        $"'{key}' is not a section that can be ordered. Use: {string.Join(", ", ResumeSections.ToKeys(ResumeSections.All))}.",
                        [validationContext.MemberName!]
                    );
                }

                if (!seen.Add(normalised))
                {
                    return new ValidationResult(
                        $"'{normalised}' is listed more than once.",
                        [validationContext.MemberName!]
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}
