// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace QuickDict
{
    public class Abbreviation : Article
    {
        public AbbreviationType AbbreviationType { get; private set; } = AbbreviationType.None;

        internal Abbreviation(DictionaryBase parent, string key, string value, AbbreviationType abbreviationType = AbbreviationType.None) : base(parent, key, value)
        {
            AbbreviationType = abbreviationType;
        }
    }

    public enum AbbreviationType
    {
        None,
        Grammatical,
        Stylistic,
        Knowledge,
        Auxiliary,
        Other
    }
}
