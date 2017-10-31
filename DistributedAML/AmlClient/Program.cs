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
                Container c = null;
                var reg = new MyRegistry(args.Any() == false ? "appsettings.json" : args[0]);
                c = new Container(reg);
                reg.For<IContainer>().Use(c);

                var db = new SQLiteConnection(reg.DataDirectory + "\\db\\client.mdb");

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

                if (false)
                {
                    var multiplexer = new Multiplexer<FuzzyWordEntry>(bucketMax + 1 /*number of buckets*/);

                    CsvReader rdr = new CsvReader(new StreamReader($@"{reg.DataDirectory}\input\Retail-Large.csv"));
                    var records = rdr.GetRecords<Retail>();
                    records.Take(1000000)
                        .Do(x => multiplexer.Add(x.Name,
                            new FuzzyWordEntry {DocId = Int32.Parse(x.Id), Phrase = x.Name}));

                    List<Task> tasks = new List<Task>();
                    multiplexer.GetBuckets().Do(x =>
                    {
                        tasks.Add(new Task(() => clientFactory.GetClient<IFuzzyMatcher>(x.Item1).AddEntry(x.Item2)));
                        tasks.Last().Start();
                    });

                    Task.WaitAll(tasks.ToArray());
                }
                else
                {
                    var data = new List<string>(new[]
                    {
                        "aleshia tomkiewicz",
                        "daniel towers",
                        "morna dick",
                        "colin dick",
                        "potatoe head",
                        "my french teacher",
                        "mrs gilbert custard",
                        "hello stranger",
                        "free fridges and gloombszyte",
                        "blue x parlour fish",
                        "aleshia x tomkiewicz",
                        "daniel x towers",
                        "morna x dick",
                        "colin x dick",
                        "potatoe x head",
                        "my french x teacher",
                        "mrs gilbert x custard",
                        "hello x stranger",
                        "free fridges x and gloombszyte",
                        "blue parlour x fish",
                        "stephen marsh",
                        "paul purbrook",
                        "michael towers",
                        "linda miskell",
                        "dan wagner",
                        "peter funnell",
                        "shane lamont",
                        "myra hindley",
                        "flash gordon",
                        "peter purves",
                        "gordon brown",
                        "tony blair",
                        "donald trump",
                        "henry kissinger",
                        "george bush",
                        "david cameron",
                        "angela leadsom",
                        "david davies",
                        "boris jonson"

                    });


                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    List<Task<List<FuzzyQueryResponse>>> tasks = new List<Task<List<FuzzyQueryResponse>>>();
                    List<Object> results = new List<object>();
                    foreach (var bucket in clientFactory.GetClientBuckets<IFuzzyMatcher>())
                    {
                        tasks.Add(Task<List<FuzzyQueryResponse>>.Factory.StartNew(() =>
                        {
                            var z = clientFactory.GetClient<IFuzzyMatcher>(bucket);

                            return z.FuzzyQuery(data);

                        }));
                    }
                    Task.WaitAll(tasks.ToArray());

                    sw.Stop();

                    Console.WriteLine($"Elapsed time - {sw.ElapsedMilliseconds}");

                    /*
                    foreach (var y in tasks)
                    {
                        foreach (var g in y.Result)
                        {
                            Console.WriteLine(g.Query);
                            foreach (var n in g.Detail)
                            {
                                Console.WriteLine($"            {n.Candidate} - {n.Score} - {n.PhraseId}");

                            }
                        }
                    }*/

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
