using System;
using System.Text.RegularExpressions;

namespace online_order_documentor_netcore.Controllers.Api
{
    public static class StringHelpers
    {
        public static string ModifyTag(this string source, string tag, Func<string, string> modifier)
        {
            string pattern = @$"(^\s*<{tag}>)(.*)(<\/{tag}>\s*$)";

            return Regex.Replace(source, pattern, m => string.Format(
                    "{0}{1}{2}",
                    m.Groups[1].Value,
                    modifier.Invoke(m.Groups[2].Value),
                    m.Groups[3].Value), RegexOptions.Multiline);
        }
    }
}
