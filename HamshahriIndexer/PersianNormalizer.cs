using System;
using SCICT.NLP.Persian;
namespace HamshahriIndexer
{
    public class PersianNormalizer
    {

        public String Normalize(String input, int len)
        {
            //PersianCharFilter filter = new PersianCharFilter();
            //String str = filter.FilterString(input);
            String str = SCICT.NLP.Utility.StringUtil.RefineAndFilterPersianWord(input);
            return str;
        }
    }
}
