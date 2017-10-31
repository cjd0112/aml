using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AmlClient.AS.Application;
using Comms;
using CsvHelper;
using Logger;
using MajordomoProtocol;
using NetMQ;
using Newtonsoft.Json;
using Shared;
using StructureMap;

namespace AmlClient
{
    class Program
    {
        public class ClientWithBucket
        {
            [PrimaryKey]
            public string ClientName { get; set; }
            public int BucketCount { get; set; }
        }


        static void Main(string[] args)
        {
            try
            {

                var s2 = Stopwatch.StartNew();
                for (int i = 0; i < 1000000; i++)
                {
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes("this is a blooming small test"));
                    var z22 = MurMurHash3.Hash(stream);
//                    var mm1 = z22 % 100;
                }
                s2.Stop();
                var q24 = s2.ElapsedMilliseconds;

                Container c = null;
                var reg = new MyRegistry(args.Any() == false ? "appsettings.json" : args[0]);
                c = new Container(reg);
                reg.For<IContainer>().Use(c);


                var databasePath = reg.DataDirectory;

                var db = new SQLiteConnection(databasePath+"\\client.mdb");

                db.CreateTable<ClientWithBucket>();

                bool clearBucketState = false;
                if (clearBucketState)
                {
                    db.Delete<ClientWithBucket>("IFuzzyMatcher");
                }

                var clientFactory = new ClientFactory(c);

                var bucketMax = clientFactory.GetClientBuckets<IFuzzyMatcher>().Max();
                var bucketMin = clientFactory.GetClientBuckets<IFuzzyMatcher>().Min();

                if (bucketMin != 0)
                    throw new Exception($"Minimum bucket is not zero it is - {bucketMin} - should be zero");

                if (db.Find<ClientWithBucket>("IFuzzyMatcher") == null)
                {
                    db.Insert(new ClientWithBucket {BucketCount = bucketMax,ClientName= "IFuzzyMatcher"});
                }
                else
                {
                    var ppp = db.Find < ClientWithBucket>("IFuzzyMatcher");
                    if (ppp.BucketCount != bucketMax)
                    {
                        throw new Exception($"We have (dynamic) bucketMax for 'IFuzzyMatcher = {bucketMax} - but last recorded run bucketMax was - {ppp.BucketCount}... cannot continue - need to match bucketCount - or rebuild");
                    }
                }

                var multiplexer = new Multiplexer<FuzzyWordEntry>(bucketMax+1 /*number of buckets*/);

                CsvReader rdr = new CsvReader(new StreamReader(@"C:\home\colin\as\input\Retail-Large.csv"));
                var records = rdr.GetRecords<Retail>();
                records.Take(100000)
                    .Do(x => multiplexer.Add(x.Name,new FuzzyWordEntry { DocId = Int32.Parse(x.Id), Phrase = x.Name }));

                List<Task> tasks = new List<Task>();
                multiplexer.GetBuckets().Do(x =>
                {
                    tasks.Add(new Task(()=>clientFactory.GetClient<IFuzzyMatcher>(x.Item1).AddEntry(x.Item2)));
                    tasks.Last().Start();
                });

                Task.WaitAll(tasks.ToArray());

                foreach (var bucket in clientFactory.GetClientBuckets<IFuzzyMatcher>())
                {
                    var z = clientFactory.GetClient<IFuzzyMatcher>(bucket);

                    if (false)
                    {
                    }
                    else
                    {
                        var q = z.FuzzyQuery(new List<string>(new[]
                            {"aleshia tomkiewicz", "daniel towers", "morna dick", "colin dick"}));

                        foreach (var g in q)
                        {
                            Console.WriteLine(g.Query);
                            foreach (var n in g.Detail)
                            {
                                Console.WriteLine($"            {n.Candidate} - {n.Score} - {n.PhraseId}");

                            }
                        }
                    }
                }
                Console.ReadLine();
                L.CloseLog();


            }
            catch (Exception e)
            {
                L.Exception(e);
            }
        }

      
    }
}
