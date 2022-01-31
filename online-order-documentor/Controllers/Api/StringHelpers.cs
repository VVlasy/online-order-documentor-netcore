using System;
using System.Text.RegularExpressions;

namespace online_order_documentor_netcore.Controllers.Api
{
    public static class StringHelpers
    {
        public static string ModifyTag(this string source, string tag, Func<string, string> modifier, string newTagName = null)
        {
            string pattern = @$"(^\s*<{tag}>)(.*)(<\/{tag}>\s*$)";

            string startTag = null;
            string closeTag = null;

            if (newTagName != null)
            {
                startTag = $"<{newTagName}>";
                closeTag = $"</{newTagName}>";
            }

            return Regex.Replace(source, pattern, m => string.Format(
                    "{0}{1}{2}",
                    startTag ?? m.Groups[1].Value,
                    modifier.Invoke(m.Groups[2].Value),
                    closeTag ?? m.Groups[3].Value), RegexOptions.Multiline);
        }

        public static string CopyTag(this string source, string tag, string newTagName)
        {
            string pattern = @$"(^\s*<{tag}>)(.*)(<\/{tag}>\s*$)";

            string startTag = newTagName ?? $"{tag}-Copy";
            string closeTag = newTagName ?? $"{tag}-Copy";

            startTag = $"<{startTag}>";
            closeTag = $"</{closeTag}>";

            return Regex.Replace(source, pattern, m => string.Format(
                    "{0}{1}{2}" + Environment.NewLine + "{3}{4}{5}",
                    m.Groups[1].Value,
                    m.Groups[2].Value,
                    m.Groups[3].Value,

                    startTag,
                    m.Groups[2].Value,
                    closeTag), RegexOptions.Multiline);
        }
    }
}
