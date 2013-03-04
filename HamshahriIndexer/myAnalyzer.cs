using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using System.IO;

namespace HamshahriIndexer
{
    public class MyAnalyzer : Analyzer 
    {
        private readonly Version _version;
        private readonly HashSet<String> _stoptable = new HashSet<String>();
        public static string DefaultStopwordFile = "PersianStopwords.txt";

        public MyAnalyzer(Version version)
        {
            _version = version;
            var fileStream = new FileStream(@"..\..\..\data\" + DefaultStopwordFile,FileMode.Open);
            if (fileStream != null)
                using (var reader = new StreamReader(fileStream))
                {
                    while (!reader.EndOfStream)
                    {
                        var word = reader.ReadLine();
                        if (word != null)
                        {
                            word = SCICT.NLP.Utility.StringUtil.RefineAndFilterPersianWord(word); // Normalize characters of stop words
                            _stoptable.Add(word);
                        }
                    }
                }
        }

        public MyAnalyzer(Version version, HashSet<String> stopWords)
        {
            _version = version;
            _stoptable = stopWords;
        }

        public override TokenStream TokenStream(string fieldname, TextReader reader)
        {
            TokenStream result = new PersianTokenizer(reader);
            result = new LowerCaseFilter(result);
            result = new PersianNormalizationFilter(result);
            result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(_version), result, _stoptable);         
            //result = new PersianLemmatizationFilter(result);
            return result;
        }
    }
    
}
