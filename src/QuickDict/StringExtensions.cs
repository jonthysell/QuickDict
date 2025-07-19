// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace QuickDict
{
    [Flags]
    public enum StringWrapInTagOptions
    {
        None = 0x0,
        WrapWholeWordsOnly = 0x1
    }

    public static class StringExtensions
    {
        public static string EscapeForXml(this string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;").Trim();
        }

        public static string WrapInTag(this string s, string tag)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return $"<{tag}>{s}</{tag}>";
        }

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
                return s.Replace(target, target.WrapInTag(tag)).Trim();
            }

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

        public static IEnumerable<string> GetDefinitions(this string value, bool keepDefinitionNumbers)
        {
            return GetDefinitions(value, keepDefinitionNumbers, 1);
        }

        private static IEnumerable<string> GetDefinitions(this string value, bool keepDefinitionNumbers, int num)
        {
            string numStr = $"{num}. ";
            string nextNumStr = $" {num + 1}. ";

            int foundIndex = value.IndexOf(numStr);
            int nextFoundIndex = value.IndexOf(nextNumStr, foundIndex + 1);

            if (num == 1 && foundIndex > 0 && nextFoundIndex > 0)
            {
                // Numbered definition with some pre-text
                if (keepDefinitionNumbers)
                {
                    yield return value.Substring(0, nextFoundIndex);
                }
                else
                {
                    yield return value.Substring(0, foundIndex) + value.Substring(foundIndex + numStr.Length, nextFoundIndex - (foundIndex + numStr.Length));
                }
            }
            else if (foundIndex == 0 && nextFoundIndex > 0)
            {
                // Numbered definition without pre-text
                if (keepDefinitionNumbers)
                {
                    yield return value.Substring(0, nextFoundIndex);
                }
                else
                {
                    yield return value.Substring(numStr.Length, nextFoundIndex - numStr.Length);
                }
            }
            else if (foundIndex == 0)
            {
                // Last numbered definition
                if (keepDefinitionNumbers)
                {
                    yield return value;
                }
                else
                {
                    yield return value.Substring(numStr.Length);
                }
            }
            else
            {
                // No numbers, just one definition
                yield return value;
            }

            if (nextFoundIndex > 0)
            {
                foreach (string def in GetDefinitions(value.Substring(nextFoundIndex + 1), keepDefinitionNumbers, num + 1))
                {
                    yield return def;
                }
            }
        }
    }
}
