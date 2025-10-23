namespace Portfolio.WebApi.Helper
{
    public class FileNameHelper
    {
        public static string FileNameFormatter(string username)
        {
            return !string.IsNullOrEmpty(username) ? username.Replace(' ', '_') + "_resume" : "resume";
        }
    }
}
