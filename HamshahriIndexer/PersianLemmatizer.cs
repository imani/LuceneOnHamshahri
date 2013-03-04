using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCICT.NLP.Morphology.Inflection;


namespace HamshahriIndexer
{
    public class PersianLemmatizer
    {
        private PersianSuffixLemmatizer lemmatizer;
        public PersianLemmatizer()
        {
            lemmatizer = new PersianSuffixLemmatizer(false, true, SCICT.NLP.Persian.Constants.PersianSuffixesCategory.ComparativeAdjectives |
                                                     SCICT.NLP.Persian.Constants.PersianSuffixesCategory.IndefiniteYaa |
                                                     SCICT.NLP.Persian.Constants.PersianSuffixesCategory.PluralSignAan |
                                                     SCICT.NLP.Persian.Constants.PersianSuffixesCategory.PluralSignHaa | 
                                                     SCICT.NLP.Persian.Constants.PersianSuffixesCategory.YaaNesbat);
        }
        public String lemmatize(String input)
        {
            var matcher = lemmatizer.MatchForSuffix(input);
            if (matcher.Length > 0)
                input = matcher[0].BaseWord;
            return input;
        }
    }
}
