namespace Portfolio.WebApi.Helper
{
    public class FileNameHelper
    {
        /// <summary>
        /// Formats a file name for a resume based on the provided name.
        /// </summary>
        /// <returns>A formatted file name replacing whitespaces with underscores, appending "_resume". E.g. "John_Doe_resume".
        /// If provided name is null or empty, returns "resume".</returns>
        public static string FileNameFormatter(string? name)
        {
            return !string.IsNullOrEmpty(name) ? name.Replace(' ', '_') + "_resume" : "resume";
        }
    }
}
