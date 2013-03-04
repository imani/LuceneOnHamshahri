using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.QueryParsers;
using System.Xml.XPath;
using Lucene.Net.Documents;
using System.IO;

namespace HamshahriSearcher
{
    struct judgement
    {
        String doc_id;
        int judge;
        public judgement(String i, int j)
        {
            doc_id = i;
            judge = j;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            //create synonyms file
            String path = @"..\..\..\data\";
            var synonyms = new Dictionary<string, List<string>>();
            StreamReader reader = new StreamReader(path + "synonyms.txt", Encoding.UTF8);
            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                if (line.Length <= 0)
                    continue;
                line = line.Trim();
                List<String> syms = line.Split(':', '،', ',').ToList<String>();
                for (int i = 0; i < syms.Count; i++)
                    syms[i] = syms[i].Trim();
                String key = syms[0];
                syms.RemoveAll(s => s.Length <= 0);
                syms.RemoveAt(0);
                if (synonyms.ContainsKey(key))
                    synonyms[key].Concat(syms);
                else
                    synonyms.Add(key, syms);
            }



            //create judgements
            int rights = 0;
            int wrongs = 0;
            Dictionary<int, List<judgement>> judges = new Dictionary<int, List<judgement>>();
            path = @"..\..\..\..\..\Hamshahri-Query_Judgement\";
            reader = new StreamReader(path + "J2.txt");
            String q = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                String[] qs = q.Split('\t',' ');
                int id = Int32.Parse(qs[0]);
                judgement j = new judgement(qs[1], Int32.Parse(qs[qs.Length-1]));
                if (judges.ContainsKey(id))
                    judges[id].Add(j);
                else
                {
                    judges.Add(id, new List<judgement>());
                    judges[id].Add(j);
                }
                q = reader.ReadLine();
                
            }

            //create searcher
            Lucene.Net.Store.Directory dir = FSDirectory.Open(@"..\..\..\LuceneIndex(simple)");
            IndexSearcher searcher = new IndexSearcher(dir, true);
            var analyzer = new HamshahriIndexer.MyAnalyzer(Lucene.Net.Util.Version.LUCENE_CURRENT);
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_CURRENT, "text", analyzer);
            
            XPathDocument xmldoc = new XPathDocument(path + "Q2.txt");
            XPathNavigator nav = xmldoc.CreateNavigator();
            XPathExpression exp = nav.Compile("//QID");
            XPathNodeIterator iter = nav.Select(exp);
            var writer = new StreamWriter(path + "simple_results.txt");
            
            //retrieve documents and compare to judges
            while(iter.MoveNext())
            {
                int id = Int32.Parse( iter.Current.Value);
                iter.Current.MoveToNext();
                String query_String = iter.Current.Value.Trim();
                String query_sym = query_String;
                // add query synonyms
                foreach (String token in query_String.Split(' ', '.', '?', '\r', '\n'))
                {
                    if (!synonyms.ContainsKey(token))
                        continue;
                    for (int i = 0; i < synonyms[token].Count && i<3; i++)
                    {
                        var syn = synonyms[token][i];
                        query_sym = query_sym + " \"" + syn + "\"";
                    }
                }
                var query = parser.Parse(query_String);
                //query.Boost = 1.5f;
                var syn_query = parser.Parse(query_sym);
                query = parser.Parse(String.Format("({0}) AND ({1})", query.ToString(), syn_query.ToString()));
                TopDocs res = searcher.Search(query, 1);
                foreach (ScoreDoc sd in res.ScoreDocs)
                {
                    var doc = searcher.Doc(sd.Doc);
                    String res_id = doc.GetField("id").StringValue.ToLower();
                    judgement j = new judgement(res_id, 1);
                    Console.Write("Document ID: " + res_id);
                    if (judges[id].Contains(j))
                    {
                        writer.WriteLine(id + "\t" + res_id + "\t"+ sd.Score +"\t1");
                        rights++;
                        Console.Write("\tCorrect\n");
                    }
                    else
                    {
                        writer.WriteLine(id + "\t" + res_id + "\t" + sd.Score + "\t0");
                        wrongs++;
                        Console.Write("\tWrong\n");
                    }
                }

                

            }

            writer.Close();
            Console.WriteLine("Precision: " + (Double)rights/(rights + wrongs));
            Console.WriteLine("Recall: " + (Double)rights/2669);

            Console.WriteLine("rights: " + rights);
            Console.WriteLine("worngs: " + wrongs);
            
            
            
        }
    }
}
