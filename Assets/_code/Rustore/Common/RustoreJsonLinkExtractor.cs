using System.Text.RegularExpressions;

namespace MegaGame
{
    public class RustoreJsonLinkExtractor
    {
        public static string AddLinksToJson(string json)
        {
            Regex urlRegex = new Regex(@"(https?://[^\s""']+)", RegexOptions.IgnoreCase);

            string result = urlRegex.Replace(json, match =>
            {
                string url = match.Value;
                url = url.TrimEnd('"', ',', ' ');
                return $"<a href=\"{url}\">{url}</a>";
            });

            return result;
        }
    }
}
