// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace QuickDict
{
    /// <summary>
    /// Options to control the behavior of <see cref="StringExtensions.WrapInTag(string,string,string,StringWrapInTagOptions)"/>.
    /// </summary>
    [Flags]
    public enum StringWrapInTagOptions
    {
        /// <summary>
        /// Wrap every literal string match of the target.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Wrap every "whole word" string match of the target.
        /// </summary>
        WrapWholeWordsOnly = 0x1
    }

    /// <summary>
    /// Extension methods for <see cref="string"/>s.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Escape XML control characters (&amp;, &lt;, &gt;) in the given string.
        /// </summary>
        /// <param name="s">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string EscapeForXml(this string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        /// <summary>
        /// Wraps the given string inside an XML element of a given tag.
        /// </summary>
        /// <param name="s">The string to wrap.</param>
        /// <param name="tag">The XML tag to wrap the string.</param>
        /// <returns>The given string wrapped in the given XML tag.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string WrapInTag(this string s, string tag)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            tag = tag.Trim();

            return $"<{tag}>{s}</{tag}>";
        }

        /// <summary>
        /// Within a given string, wrap every instance of the target inside an XML element of a given tag.
        /// </summary>
        /// <param name="s">The string to search.</param>
        /// <param name="target">The target string that should be wrapped in the XML tag.</param>
        /// <param name="tag">The XML tag to wrap each target string.</param>
        /// <param name="options">Options to control when wrapping occurs.</param>
        /// <returns>The given string where each target string is wrapped in the  given XML tag.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string WrapInTag(this string s, string target, string tag, StringWrapInTagOptions options = StringWrapInTagOptions.None)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (string.IsNullOrWhiteSpace(target))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (options == StringWrapInTagOptions.None)
            {
                return s.Replace(target, target.WrapInTag(tag));
            }

            tag = tag.Trim();

            s = s.Replace($" {target} ", $" <{tag}>{target}</{tag}> ");
            s = s.Replace($"({target} ", $"(<{tag}>{target}</{tag}> ");
            s = s.Replace($" {target})", $" <{tag}>{target}</{tag}>)");
            s = s.Replace($"({target})", $"(<{tag}>{target}</{tag}>)");

            s = s.Replace($"{target}.", $"<{tag}>{target}</{tag}>.");

            s = s.Replace($"{target};", $"<{tag}>{target}</{tag}>;");

            s = s.Replace($"{target},", $"<{tag}>{target}</{tag}>,");
            s = s.Replace($"({target},", $"(<{tag}>{target}</{tag}>,");

            s = s.Replace($"{target}/", $"<{tag}>{target}</{tag}>/");
            s = s.Replace($"/{target}", $"/<{tag}>{target}</{tag}>");

            s = s.Replace($"—{target} ", $"—<{tag}>{target}</{tag}> ");
            s = s.Replace($" {target}—", $" <{tag}>{target}</{tag}>—");

            if (s.StartsWith(target + " "))
            {
                s = $"<{tag}>{target}</{tag}>{s.Substring(target.Length)}";
            }

            if (s.EndsWith(" " + target))
            {
                s = $"{s.Substring(0, s.Length - target.Length)}<{tag}>{target}</{tag}>";
            }

            return s;
        }

        /// <summary>
        /// Divide the given string by numbered definition.
        /// </summary>
        /// <param name="s">The given string.</param>
        /// <param name="keepDefinitionNumbers">Whether or not to keep the defintion numbers in the result.</param>
        /// <returns>Each separate definition, in order.</returns>
        public static IEnumerable<string> GetDefinitions(this string s, bool keepDefinitionNumbers)
        {
            return GetDefinitions(s, keepDefinitionNumbers, 1);
        }

        private static IEnumerable<string> GetDefinitions(this string s, bool keepDefinitionNumbers, int num)
        {
            string numStr = $"{num}. ";
            string nextNumStr = $" {num + 1}. ";

            int foundIndex = s.IndexOf(numStr);
            int nextFoundIndex = s.IndexOf(nextNumStr, foundIndex + 1);

            if (num == 1 && foundIndex > 0 && nextFoundIndex > 0)
            {
                // Numbered definition with some pre-text
                if (keepDefinitionNumbers)
                {
                    yield return s.Substring(0, nextFoundIndex);
                }
                else
                {
                    yield return s.Substring(0, foundIndex) + s.Substring(foundIndex + numStr.Length, nextFoundIndex - (foundIndex + numStr.Length));
                }
            }
            else if (foundIndex == 0 && nextFoundIndex > 0)
            {
                // Numbered definition without pre-text
                if (keepDefinitionNumbers)
                {
                    yield return s.Substring(0, nextFoundIndex);
                }
                else
                {
                    yield return s.Substring(numStr.Length, nextFoundIndex - numStr.Length);
                }
            }
            else if (foundIndex == 0)
            {
                // Last numbered definition
                if (keepDefinitionNumbers)
                {
                    yield return s;
                }
                else
                {
                    yield return s.Substring(numStr.Length);
                }
            }
            else
            {
                // No numbers, just one definition
                yield return s;
            }

            if (nextFoundIndex > 0)
            {
                foreach (string def in GetDefinitions(s.Substring(nextFoundIndex + 1), keepDefinitionNumbers, num + 1))
                {
                    yield return def;
                }
            }
        }
    }
}
