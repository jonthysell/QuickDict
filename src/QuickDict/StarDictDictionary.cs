// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuickDict
{
    /// <summary>
    /// Creates a dictionary in the StarDict format.
    /// </summary>
    public class StarDictDictionary : DictionaryBase
    {
        /// <summary>
        /// Hook to provide synonyms for a given <see cref="Article" />.
        /// </summary>
        public Func<Article, ISet<string>> GetStarDictSynonymsFromArticle { get; set; } = null;

        private static readonly StarDictArticleComparer _keyComparer = new StarDictArticleComparer();

        /// <summary>
        /// Initializes a new instance of the <see cref="StarDictDictionary"/> class.
        /// </summary>
        /// <param name="metadata">The dictionary's starting metadata.</param>
        public StarDictDictionary(DictionaryMetadata metadata = null) : base(metadata) { }

        /// <summary>
        /// Save this <see cref="StarDictDictionary" /> to the given (ifo) filename, with other files saved in the same path.
        /// </summary>
        /// <param name="filename">The (ifo) filename of the file to save to.</param>
        public override void Save(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (!Path.HasExtension(filename))
            {
                filename += ".ifo";
            }

            string ifoFile = Path.ChangeExtension(filename, ".ifo");
            string dictFile = Path.ChangeExtension(filename, ".dict");
            string idxFile = Path.ChangeExtension(filename, ".idx");
            string synFile = Path.ChangeExtension(filename, ".syn");

            using var ifoStream = new FileStream(ifoFile, FileMode.Create);
            using var dictStream = new FileStream(dictFile, FileMode.Create);
            using var idxStream = new FileStream(idxFile, FileMode.Create);
            using var synStream = new FileStream(synFile, FileMode.Create);

            Save(ifoStream, dictStream, idxStream, synStream);
        }

        /// <summary>
        /// Save this <see cref="StarDictDictionary" /> to the given streams.
        /// </summary>
        /// <param name="ifoStream">The stream for the StarDict ifo file.</param>
        /// <param name="dictStream">The stream for the StarDict dict file.</param>
        /// <param name="idxStream">The stream for the StarDict idx file.</param>
        /// <param name="synStream">The stream for the StarDict syn file.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Save(Stream ifoStream, Stream dictStream, Stream idxStream, Stream synStream)
        {
            if (ifoStream is null)
            {
                throw new ArgumentNullException(nameof(ifoStream));
            }

            if (dictStream is null)
            {
                throw new ArgumentNullException(nameof(dictStream));
            }

            if (idxStream is null)
            {
                throw new ArgumentNullException(nameof(idxStream));
            }

            if (synStream is null)
            {
                throw new ArgumentNullException(nameof(synStream));
            }

            var dictWriter = new BinaryWriter(dictStream, Encoding.UTF8);
            var idxWriter = new BinaryWriter(idxStream, Encoding.UTF8);
            var synWriter = new BinaryWriter(synStream, Encoding.UTF8);

            var articleIndexes = new Dictionary<Article, uint>();

            uint index = 0;
            foreach (var article in Articles.OrderBy(a => GetKeyFromArticle is not null ? GetKeyFromArticle(a) : a.Key, _keyComparer))
            {
                long dictArticleOffset = dictWriter.BaseStream.Length;

                idxWriter.Write((GetKeyFromArticle is not null ? GetKeyFromArticle(article) : article.Key).Trim().ToCharArray());
                idxWriter.Write('\0');

                dictWriter.Write((GetValueFromArticle is not null ? GetValueFromArticle(article) : article.Value).Trim().ToCharArray());

                dictWriter.Flush();

                long dictArticleLength = dictWriter.BaseStream.Length - dictArticleOffset;

                WriteBigEndian(idxWriter, (uint)dictArticleOffset);
                WriteBigEndian(idxWriter, (uint)dictArticleLength);

                idxWriter.Flush();

                articleIndexes[article] = index;
                index++;
            }

            dictWriter.Flush();
            dictWriter.Close();

            idxWriter.Flush();
            long idxFileSize = idxWriter.BaseStream.Length;
            idxWriter.Close();

            var synonyms = new List<KeyValuePair<string, uint>>();

            foreach (var articleIndex in articleIndexes)
            {
                uint keyIndex = articleIndex.Value;

                var rawKey = (GetKeyFromArticle is not null ? GetKeyFromArticle(articleIndex.Key) : articleIndex.Key.Key).Trim();

                var rawSynonyms = GetStarDictSynonymsFromArticle?.Invoke(articleIndex.Key);
                rawSynonyms?.Remove(rawKey); // Synonyms shouldn't contain the original key

                foreach (string rawSynonym in rawSynonyms)
                {
                    if (!string.IsNullOrWhiteSpace(rawSynonym))
                    {
                        synonyms.Add(new KeyValuePair<string, uint>(rawSynonym.Trim(), keyIndex));
                    }
                }
            }

            foreach (var synonym in synonyms.OrderBy(kvp => kvp.Key, _keyComparer))
            {
                synWriter.Write(synonym.Key.ToCharArray());
                synWriter.Write('\0');

                WriteBigEndian(synWriter, synonym.Value);
            }

            int synWordCount = synonyms.Count;

            synWriter.Flush();
            synWriter.Close();

            using var ifoWriter = new BinaryWriter(ifoStream, Encoding.UTF8);

            WriteLine(ifoWriter, "StarDict's dict ifo file");
            WriteLine(ifoWriter, "version=2.4.2");

            WriteLine(ifoWriter, "bookname={0}", Metadata.LongTitle);
            WriteLine(ifoWriter, "wordcount={0}", Articles.Count);
            WriteLine(ifoWriter, "synwordcount={0}", synWordCount);
            WriteLine(ifoWriter, "idxfilesize={0}", idxFileSize);
            WriteLine(ifoWriter, "sametypesequence=h");

            WriteLine(ifoWriter, "author={0}", string.Join(", ", Metadata.Authors));
            WriteLine(ifoWriter, "description={0}", Metadata.Description);
            WriteLine(ifoWriter, "date={0}", Metadata.CreationDateTime.ToString("yyyy.MM.dd"));
        }

        private static void WriteLine(BinaryWriter bw, string line, params object[] args)
        {
            bw.Write(string.Format(line, args).ToCharArray());
            bw.Write('\r');
            bw.Write('\n');
        }

        private static void WriteBigEndian(BinaryWriter bw, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            bw.Write(bytes);
        }

        private class StarDictArticleComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int result = AsciiStrCmp(x, y);
                return result == 0 ? StrCmp(x, y) : result;
            }

            private static int AsciiStrCmp(string x, string y)
            {
                int[] bx = Encoding.UTF8.GetBytes(x).Select(b => (int)(b)).ToArray();
                int[] by = Encoding.UTF8.GetBytes(y).Select(b => (int)(b)).ToArray();

                int minLength = Math.Min(bx.Length, by.Length);

                for (int i = 0; i < minLength; i++)
                {
                    int cx = AsciiLower(bx[i]);
                    int cy = AsciiLower(by[i]);

                    if (cx != cy)
                    {
                        return cx - cy;
                    }
                }

                return bx.Length - by.Length;
            }

            private static int AsciiLower(int c)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    return (c - 'A' + 'a');
                }
                return c;
            }

            private static int StrCmp(string x, string y)
            {
                int[] bx = Encoding.UTF8.GetBytes(x).Select(b => (int)(b)).ToArray();
                int[] by = Encoding.UTF8.GetBytes(y).Select(b => (int)(b)).ToArray();

                int minLength = Math.Min(bx.Length, by.Length);

                for (int i = 0; i < minLength; i++)
                {
                    int cx = bx[i];
                    int cy = by[i];

                    if (cx != cy)
                    {
                        return cx - cy;
                    }
                }

                return bx.Length - by.Length;
            }
        }
    }
}