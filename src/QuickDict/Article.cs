// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace QuickDict
{
    /// <summary>
    /// An entry (term and defintion) in a <see cref="DictionaryBase" />.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// The <see cref="DictionaryBase" /> this <see cref="Article" /> is a part of.
        /// </summary>
        public readonly DictionaryBase Parent;

        /// <summary>
        /// The term of this <see cref="Article" />.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The definition of this <see cref="Article" />.
        /// </summary>
        public readonly string Value;

        internal Article(DictionaryBase parent, string key, string value)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));

            Key = !string.IsNullOrWhiteSpace(key) ? key.Trim() : throw new ArgumentNullException(nameof(key));
            Value = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentNullException(nameof(value));
        }
    }
}
