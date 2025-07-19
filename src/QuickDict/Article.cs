// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace QuickDict
{
    public class Article
    {
        public readonly DictionaryBase Parent;

        public readonly string Key;

        public readonly string Value;

        internal Article(DictionaryBase parent, string key, string value)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));

            Key = !string.IsNullOrWhiteSpace(key) ? key.Trim() : throw new ArgumentNullException(nameof(key));
            Value = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentNullException(nameof(value));
        }
    }
}
