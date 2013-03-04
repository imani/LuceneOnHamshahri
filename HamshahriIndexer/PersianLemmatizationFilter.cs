using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace HamshahriIndexer
{
    class PersianLemmatizationFilter : TokenFilter
    {
        private PersianLemmatizer lemmatizer;
        private readonly TermAttribute _termAtt;

        public PersianLemmatizationFilter(TokenStream input)
            : base(input)
        {
            lemmatizer = new PersianLemmatizer();
            _termAtt = (TermAttribute)input.AddAttribute<ITermAttribute>();
        }

        public override bool IncrementToken()
        {
            if (input.IncrementToken())
            {
                string lemmatized = lemmatizer.lemmatize(_termAtt.Term); 
                _termAtt.SetTermBuffer(lemmatized);
                return true;
            }
            return false;
        }
    }
}
