// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace QuickDict
{
    /// <summary>
    /// Base class for a dictionary.
    /// </summary>
    public abstract class DictionaryBase
    {
        /// <summary>
        /// The <see cref="DictionaryMetadata" /> for this <see cref="DictionaryBase" />.
        /// </summary>
        public DictionaryMetadata Metadata { get; private set; }

        /// <summary>
        /// The list of <see cref="Article" />s defined for this <see cref="DictionaryBase" />.
        /// </summary>
        public IReadOnlyList<Article> Articles => _articles;
        private readonly List<Article> _articles = new List<Article>();

        /// <summary>
        /// Hook to override the behavior when trying to get an <see cref="Article" />'s <see cref="Article.Key" />.
        /// </summary>
        public Func<Article, string> GetKeyFromArticle { get; set; } = null;

        /// <summary>
        /// Hook to override the behavior when trying to get an <see cref="Article" />'s <see cref="Article.Value" />.
        /// </summary>
        public Func<Article, string> GetValueFromArticle { get; set; } = null;

        /// <summary>
        /// Hook to override the behavior when trying to get an <see cref="Abbreviation" />'s <see cref="Abbreviation.Key" />.
        /// </summary>
        public Func<Abbreviation, string> GetKeyFromAbbreviation { get; set; } = null;

        /// <summary>
        /// Hook to override the behavior when trying to get an <see cref="Abbreviation" />'s <see cref="Abbreviation.Key" />.
        /// </summary>
        public Func<Abbreviation, string> GetValueFromAbbreviation { get; set; } = null;

        /// <summary>
        /// The list of <see cref="Abbreviation" />s defined for this <see cref="DictionaryBase" />.
        /// </summary>
        public IReadOnlyList<Abbreviation> Abbreviations => _abbreviations;
        private readonly List<Abbreviation> _abbreviations = new List<Abbreviation>();

        internal DictionaryBase(DictionaryMetadata metadata = null)
        {
            Metadata = metadata ?? new DictionaryMetadata();
        }

        /// <summary>
        /// Save this <see cref="DictionaryBase" /> to the given filename.
        /// </summary>
        /// <param name="filename">The filename of the file to save to.</param>
        public abstract void Save(string filename);

        /// <summary>
        /// Adds a new <see cref="Article" /> to the <see cref="DictionaryBase" /> with the given key and value.
        /// </summary>
        /// <param name="key">The <see cref="Article" />'s key.</param>
        /// <param name="value">The <see cref="Article" />'s value.</param>
        public void AddArticle(string key, string value)
        {
            var article = new Article(this, key, value);
            _articles.Add(article);
        }

        /// <summary>
        /// Adds a new <see cref="Abbreviation" /> to the <see cref="DictionaryBase" /> with the given key and value.
        /// </summary>
        /// <param name="key">The <see cref="Abbreviation" />'s key.</param>
        /// <param name="value">The <see cref="Abbreviation" />'s value.</param>
        /// <param name="abbreviationType">The <see cref="Abbreviation" />'s <see cref="AbbreviationType" />.</param>
        public void AddAbbreviation(string key, string value, AbbreviationType abbreviationType = AbbreviationType.None)
        {
            var abbreviation = new Abbreviation(this, key, value, abbreviationType);
            _abbreviations.Add(abbreviation);
        }
    }
}
