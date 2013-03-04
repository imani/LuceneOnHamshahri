using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HamshahriIndexer
{
    class PersianTokenizer : Lucene.Net.Analysis.WhitespaceTokenizer
    {
        public PersianTokenizer(System.IO.TextReader input)
            : base(input)
        {
        }

        protected override bool IsTokenChar(char c)
        {
            return !char.IsWhiteSpace(c) && !char.IsPunctuation(c) && !char.IsSymbol(c);
        }
    }
}
