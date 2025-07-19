// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace QuickDict
{
    public abstract class DictionaryBase
    {
        public DictionaryMetadata Metadata { get; private set; }

        public IReadOnlyList<Article> Articles => _articles;
        private readonly List<Article> _articles = new List<Article>();

        public Func<Article, string> GetKeyFromArticle { get; set; } = null;

        public Func<Article, string> GetValueFromArticle { get; set; } = null;

        public Func<Abbreviation, string> GetKeyFromAbbreviation { get; set; } = null;

        public Func<Abbreviation, string> GetValueFromAbbreviation { get; set; } = null;

        public IReadOnlyList<Abbreviation> Abbreviations => _abbreviations;
        private readonly List<Abbreviation> _abbreviations = new List<Abbreviation>();

        public DictionaryBase(DictionaryMetadata metadata = null)
        {
            Metadata = metadata ?? new DictionaryMetadata();
        }

        public abstract void Save(string filename);

        public void AddArticle(string key, string value)
        {
            var article = new Article(this, key, value);
            _articles.Add(article);
        }

        public void AddAbbreviation(string key, string value, AbbreviationType abbreviationType = AbbreviationType.None)
        {
            var abbreviation = new Abbreviation(this, key, value, abbreviationType);
            _abbreviations.Add(abbreviation);
        }
    }

    public class DictionaryMetadata
    {
        public string ShortTitle { get; set; } = null;

        public string LongTitle { get; set; } = null;

        public string Description { get; set; } = null;

        public string ArticleKeyLangCode { get; set; } = null;

        public string ArticleValueLangCode { get; set; } = null;

        public DateTime CreationDateTime { get; private set; } = DateTime.UtcNow;

        public List<string> Authors { get; private set; } = new List<string>();

        public string SrcUrl { get; set; } = null;

        public string FileVersion { get; set; } = null;

        public DictionaryMetadata() { }
    }
}
