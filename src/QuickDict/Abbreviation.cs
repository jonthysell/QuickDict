// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace QuickDict
{
    /// <summary>
    /// An abbreviation (term and defintion) used in a <see cref="DictionaryBase" />.
    /// </summary>
    public class Abbreviation
    {
        /// <summary>
        /// The <see cref="DictionaryBase" /> this <see cref="Abbreviation" /> is a part of.
        /// </summary>
        public readonly DictionaryBase Parent;

        /// <summary>
        /// The term of this <see cref="Abbreviation" />.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The definition of this <see cref="Abbreviation" />.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The type of this <see cref="Abbreviation" />.
        /// </summary>
        public AbbreviationType AbbreviationType { get; private set; } = AbbreviationType.None;

        internal Abbreviation(DictionaryBase parent, string key, string value, AbbreviationType abbreviationType = AbbreviationType.None)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));

            Key = !string.IsNullOrWhiteSpace(key) ? key.Trim() : throw new ArgumentNullException(nameof(key));
            Value = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentNullException(nameof(value));
            AbbreviationType = abbreviationType;
        }
    }

    /// <summary>
    /// The type of an <see cref="Abbreviation" />, typically only used by <see cref="XdxfDictionary" />.
    /// </summary>
    public enum AbbreviationType
    {
        /// <summary>
        /// The <see cref="Abbreviation" /> has no type.
        /// </summary>
        None,
        /// <summary>
        /// The <see cref="Abbreviation" /> states a grammatical feature of a word (noun, past participle, etc.).
        /// </summary>
        Grammatical,
        /// <summary>
        /// The <see cref="Abbreviation" /> is a stylistic property of a word (vulgar, archaic, obsolete, poetic, disapproving etc.).
        /// </summary>
        Stylistic,
        /// <summary>
        /// The <see cref="Abbreviation" /> is an area/field of knowledge (computers, literature, culinary, typography etc.).
        /// </summary>
        Knowledge,
        /// <summary>
        /// The <see cref="Abbreviation" /> is a simple subsidiary word like ('e.g.', 'i.e.', 'cf.', 'also', 'rare' etc.).
        /// </summary>
        Auxiliary,
        /// <summary>
        /// The <see cref="Abbreviation" /> is of another type.
        /// </summary>
        Other
    }
}
