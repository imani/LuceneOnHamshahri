using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Analysis;

namespace HamshahriIndexer
{
    public sealed class PersianNormalizationFilter : TokenFilter
    {
        private readonly PersianNormalizer _normalizer;
        private readonly TermAttribute _termAtt;

        public PersianNormalizationFilter(TokenStream input)
            : base(input)
        {
            _normalizer = new PersianNormalizer();
            _termAtt = (TermAttribute)input.AddAttribute<ITermAttribute>();
        }

        public override bool IncrementToken()
        {
            //if (_termAtt.Term.Length <= 2)
            //    return false;
            if (input.IncrementToken())
            {
                string normalized =  _normalizer.Normalize(_termAtt.Term, _termAtt.TermLength());
                _termAtt.SetTermBuffer(normalized);
                return true;
            }
            return false;
        }
    }
}
