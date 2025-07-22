// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace QuickDict
{
    /// <summary>
    /// Creates a dictionary in the XDXF format.
    /// </summary>
    public class XdxfDictionary : DictionaryBase
    {
        /// <summary>
        /// Hook to provide the behavior to derive multiple keys from a single <see cref="Article" />.
        /// </summary>
        public Func<Article, IList<string>> GetXdxfKeysFromArticle { get; set; } = null;

        /// <summary>
        /// Hook to provide the behavior to derive multiple values from a single <see cref="Article" />.
        /// </summary>
        public Func<Article, IList<string>> GetXdxfValuesFromArticle { get; set; } = null;

        /// Hook to provide optional terms which can be ignored in an <see cref="Article" />'s <see cref="Article.Key" />.
        public Func<ISet<string>> GetXdxfKeyOptionalTerms { get; set; } = null;

        /// <summary>
        /// Hook to provide the behavior to derive multiple keys from a single <see cref="Abbreviation" />.
        /// </summary>
        public Func<Abbreviation, IList<string>> GetXdxfKeysFromAbbreviation { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="XdxfDictionary"/> class.
        /// </summary>
        /// <param name="metadata">The dictionary's starting metadata.</param>
        public XdxfDictionary(DictionaryMetadata metadata = null) : base(metadata) { }

        /// <summary>
        /// Save this <see cref="XdxfDictionary"/> to the five (xdxf) filename.
        /// </summary>
        /// <param name="filename">The (xdxf) filename fo the file to save to.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void Save(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (!Path.HasExtension(filename) || Path.GetExtension(filename).ToLowerInvariant() != ".xdxf")
            {
                filename += ".xdxf";
            }

            using var fs = new FileStream(filename, FileMode.Create);

            Save(fs);
        }

        /// <summary>
        /// Save this <see cref="XdxfDictionary" /> to the given stream.
        /// </summary>
        /// <param name="output">The stream for the xdxf file.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Save(Stream output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            // Write to StringBuilder
            var sb = new StringBuilder();
            using (var xw = XmlWriter.Create(sb, new XmlWriterSettings() { Encoding = Encoding.UTF8, CloseOutput = false }))
            {
                xw.WriteStartDocument();

                xw.WriteStartElement("xdxf");

                xw.WriteAttributeString("format", "logical");
                xw.WriteAttributeString("revision", "33");
                WriteAttributeStringIfNotNull(xw, "lang_from", Metadata.ArticleKeyLangCode);
                WriteAttributeStringIfNotNull(xw, "lang_to", Metadata.ArticleValueLangCode);

                WriteMetaInfoElements(xw);

                WriteArticles(xw);

                xw.WriteEndElement(); // xdxf

                xw.WriteEndDocument();
            }

            // Load from StringBuilder
            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            // Write to stream
            using (XmlWriter xw = XmlWriter.Create(output, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true, CloseOutput = false }))
            {
                doc.Save(xw);
            }
        }

        private void WriteMetaInfoElements(XmlWriter xw)
        {
            xw.WriteStartElement("meta_info");

            WriteElementStringIfNotNull(xw, "title", Metadata.ShortTitle);

            WriteElementStringIfNotNull(xw, "full_title", Metadata.LongTitle);

            if (Metadata.Authors.Count > 0)
            {
                xw.WriteStartElement("authors");

                foreach (string author in Metadata.Authors)
                {
                    WriteElementStringIfNotNull(xw, "author", author);
                }

                xw.WriteEndElement(); // authors
            }

            WriteElementStringIfNotNull(xw, "description", Metadata.Description);

            WriteAbbreviations(xw);

            WriteElementStringIfNotNull(xw, "file_ver", Metadata.FileVersion);

            WriteElementStringIfNotNull(xw, "creation_date", Metadata.CreationDateTime.Date.ToString("dd-MM-yyyy"));

            WriteElementStringIfNotNull(xw, "dict_src_url", Metadata.SrcUrl);

            xw.WriteEndElement(); // meta_info
        }

        private void WriteAbbreviations(XmlWriter xw)
        {
            if (Abbreviations.Count > 0)
            {
                xw.WriteStartElement("abbreviations");

                foreach (var abbreviation in Abbreviations)
                {
                    xw.WriteStartElement("abbr_def");

                    switch (abbreviation.AbbreviationType)
                    {
                        case AbbreviationType.Grammatical:
                            xw.WriteAttributeString("type", "grm");
                            break;
                        case AbbreviationType.Stylistic:
                            xw.WriteAttributeString("type", "stl");
                            break;
                        case AbbreviationType.Knowledge:
                            xw.WriteAttributeString("type", "knl");
                            break;
                        case AbbreviationType.Auxiliary:
                            xw.WriteAttributeString("type", "aux");
                            break;
                        case AbbreviationType.Other:
                            xw.WriteAttributeString("type", "oth");
                            break;
                    }

                    xw.WriteRaw(GetWrappedAbbreviationKey(abbreviation).Trim());
                    xw.WriteRaw(GetWrappedAbbreviationValue(abbreviation).Trim());

                    xw.WriteEndElement(); // abbr_def
                }

                xw.WriteEndElement(); // abbreviations
            }
        }

        private string GetWrappedAbbreviationKey(Abbreviation abbreviation)
        {
            var rawKeys = GetXdxfKeysFromAbbreviation is not null ? GetXdxfKeysFromAbbreviation(abbreviation) : new List<string>() { GetKeyFromAbbreviation is not null ? GetKeyFromAbbreviation(abbreviation) : abbreviation.Key.EscapeForXml() };

            var wrappedKeySB = new StringBuilder();
            foreach (var rawKey in rawKeys)
            {
                if (!string.IsNullOrWhiteSpace(rawKey))
                {
                    wrappedKeySB.Append(rawKey.WrapInTag("abbr_k"));
                }
            }

            return wrappedKeySB.ToString();
        }

        private string GetWrappedAbbreviationValue(Abbreviation abbreviation)
        {
            return (GetValueFromAbbreviation is not null ? GetValueFromAbbreviation(abbreviation) : abbreviation.Value.EscapeForXml()).WrapInTag("abbr_v");
        }

        private void WriteArticles(XmlWriter xw)
        {
            if (Articles.Count > 0)
            {
                xw.WriteStartElement("lexicon");

                foreach (var article in Articles)
                {
                    xw.WriteStartElement("ar");

                    xw.WriteRaw(GetWrappedArticleKey(article));
                    xw.WriteRaw(GetWrappedArticleValue(article));

                    xw.WriteEndElement(); // ar
                }

                xw.WriteEndElement(); // lexicon
            }
        }

        private string GetWrappedArticleKey(Article article)
        {
            // Break down the single key into multiple keys if possible
            var rawKeys = GetXdxfKeysFromArticle is not null ? GetXdxfKeysFromArticle(article) : new List<string>() { GetKeyFromArticle is not null ? GetKeyFromArticle(article) : article.Key.EscapeForXml() };

            // Get every key properly wrapped with optional terms also tagged
            var wrappedKeySB = new StringBuilder();
            foreach (var rawKey in rawKeys)
            {
                if (!string.IsNullOrWhiteSpace(rawKey))
                {
                    var result = rawKey;

                    // Add opt around optional terms
                    if (GetXdxfKeyOptionalTerms is not null)
                    {
                        foreach (var optionalTerm in GetXdxfKeyOptionalTerms())
                        {
                            result = result.WrapInTag(optionalTerm, "opt");
                        }
                    }

                    wrappedKeySB.Append(result.WrapInTag("k"));
                }
            }

            return wrappedKeySB.ToString();
        }

        private string GetWrappedArticleValue(Article article)
        {
            var rawValues = GetXdxfValuesFromArticle is not null ? GetXdxfValuesFromArticle(article) : new List<string>() { GetValueFromArticle is not null ? GetValueFromArticle(article) : article.Value.EscapeForXml() };

            // Get every value properly wrapped with abbreviations tagged
            var wrappedValueSB = new StringBuilder();
            foreach (var rawValue in rawValues)
            {
                string result = rawValue;

                // Add abbreviation tags
                foreach (var abbreviation in GetXdxfKeysFromAbbreviation is not null ? Abbreviations.SelectMany(GetXdxfKeysFromAbbreviation) : Abbreviations.Select(a => GetKeyFromAbbreviation is not null ? GetKeyFromAbbreviation(a) : a.Key.EscapeForXml()))
                {
                    result = result.WrapInTag(abbreviation, "abbr", StringWrapInTagOptions.WrapWholeWordsOnly);
                }

                wrappedValueSB.Append(result.WrapInTag("deftext").WrapInTag("def"));
            }

            return rawValues.Count > 1 ? wrappedValueSB.ToString().WrapInTag("def") : wrappedValueSB.ToString();
        }

        private static void WriteElementStringIfNotNull(XmlWriter xw, string localName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                xw.WriteElementString(localName, value.Trim());
            }
        }

        private static void WriteAttributeStringIfNotNull(XmlWriter xw, string localName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                xw.WriteAttributeString(localName, value.Trim());
            }
        }
    }
}
