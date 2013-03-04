using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using System.Xml;
using Lucene.Net.Documents;


namespace HamshahriIndexer
{
    class Program
    {
        static void Main(string[] args)
        {

            Lucene.Net.Store.Directory index_dir = FSDirectory.Open(@"..\..\..\LuceneIndex(simple)");
            Analyzer analyzer = new MyAnalyzer(Lucene.Net.Util.Version.LUCENE_CURRENT);
            IndexWriter writer = new IndexWriter(index_dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            //reading files
            String path = @"..\..\..\..\separated\";
            DirectoryInfo dir = new DirectoryInfo(path);
            int counter = 0;
            foreach (FileInfo file in dir.GetFiles())
            {
                StreamReader reader = new StreamReader(file.FullName);
                if (file.Extension != ".txt")
                    continue;
                Document doc = new Document();
                Field id = new Field("id", file.Name.Substring(0,file.Name.Length-4), Field.Store.YES, Field.Index.NO);
                Field date = null;
                Field cat = null;
                Field text = null;
                String line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    if (line.Contains(".DID"))
                    {
                        doc.Add(id);
                        line = reader.ReadLine();
                    }
                    else if (line.Contains(".Date"))
                    {
                        date = new Field("date", line.Substring(6), Field.Store.YES, Field.Index.NO);
                        line = reader.ReadLine();
                        doc.Add(date);
                    }
                    else if (line.Contains(".Cat"))
                    {
                        cat = new Field("cat", line.Substring(5), Field.Store.YES, Field.Index.NOT_ANALYZED);
                        line = reader.ReadToEnd();
                        text = new Field("text", line, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                        doc.Add(cat);
                        doc.Add(text);
                        Console.WriteLine("Document with ID:" + id.StringValue + " indexed.");
                        writer.AddDocument(doc);
                        reader.Close();
                        break;
                    }
                    else
                        line = reader.ReadLine();

                }

            }
            writer.Optimize();
            writer.Commit();
            writer.Dispose();






        }
    }
}
