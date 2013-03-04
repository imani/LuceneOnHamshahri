using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Analysis;

namespace HamshahriIndexer
{
    public sealed class PersianStemFilter : TokenFilter
    {
        private readonly PersianStemmer _stemmer;
        private readonly TermAttribute _termAttr;

        public PersianStemFilter(TokenStream input) : base(input)
        {
            _stemmer = new PersianStemmer();
            _termAttr = AddAttribute<TermAttribute>();
        }

        public override bool IncrementToken()
        {
            if (input.IncrementToken())
            {
                var newLength = _stemmer.Stem(_termAttr.TermBuffer(), _termAttr.TermLength());
                _termAttr.SetTermLength(newLength);
                return true;
            }

            return false;
        }
    }
}
