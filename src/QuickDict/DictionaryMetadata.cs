// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace QuickDict
{
    /// <summary>
    /// The metadata for a <see cref="DictionaryBase" />.
    /// </summary>
    public class DictionaryMetadata
    {
        /// <summary>
        /// The short title of a <see cref="DictionaryBase" />.
        /// </summary>
        public string ShortTitle { get; set; } = null;

        /// <summary>
        /// The full or long title of a <see cref="DictionaryBase" />.
        /// </summary>
        public string LongTitle { get; set; } = null;

        /// <summary>
        /// The description of a <see cref="DictionaryBase" />.
        /// </summary>
        public string Description { get; set; } = null;

        /// <summary>
        /// The ISO 3-letter language code for the <see cref="Article" /> keys (terms) of a <see cref="DictionaryBase" />, i.e. the "from" language.
        /// </summary>
        public string ArticleKeyLangCode { get; set; } = null;

        /// <summary>
        /// The ISO 3-letter language code for the <see cref="Article" /> values (definitions) of a <see cref="DictionaryBase" />, i.e. the "to" language.
        /// </summary>
        public string ArticleValueLangCode { get; set; } = null;

        /// <summary>
        /// The creation timestamp for a <see cref="DictionaryBase" />.
        /// </summary>
        public DateTime CreationDateTime { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// The authors of a <see cref="DictionaryBase" />.
        /// </summary>
        public List<string> Authors { get; private set; } = new List<string>();

        /// <summary>
        /// The source URL for a <see cref="DictionaryBase" />.
        /// </summary>
        public string SrcUrl { get; set; } = null;

        /// <summary>
        /// The file version of a <see cref="DictionaryBase" />.
        /// </summary>
        public string FileVersion { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMetadata"/> class.
        /// </summary>
        public DictionaryMetadata() { }
    }
}
