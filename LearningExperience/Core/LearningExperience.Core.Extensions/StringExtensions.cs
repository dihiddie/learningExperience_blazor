namespace LearningExperience.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string WrapWordsInTag(this string originalText, List<string> searchWords, string tag)
        {
            var pattern = string.Join("|", searchWords.Select(Regex.Escape));
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var matches = regex.Matches(originalText);
            return matches.Count <= 0
                       ? originalText
                       : matches.GroupBy(m => m.Value).Select(ind => ind.First()).Aggregate(
                           originalText,
                           (current, match) => current.Replace(match.Value, $"<{tag}>{match}</{tag}>"));
        }

        public static bool CaseInsensitiveContains(
            this string text,
            string value,
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase) =>
            text.IndexOf(value, stringComparison) >= 0;
    }
}
