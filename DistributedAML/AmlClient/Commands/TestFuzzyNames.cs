using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comms;

namespace AmlClient.Commands
{
    public class TestFuzzyNames : AmlCommand
    {
        private ClientFactory factory;
        public TestFuzzyNames(ClientFactory factory)
        {
            this.factory = factory;

        }
        public override void Run()
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
            List<Task<IEnumerable<FuzzyQueryResponse>>> tasks = new List<Task<IEnumerable<FuzzyQueryResponse>>>();
            List<Object> results = new List<object>();
            foreach (var bucket in factory.GetClientBuckets<IFuzzyMatcher>())
            {
                tasks.Add(Task<IEnumerable<FuzzyQueryResponse>>.Factory.StartNew(() =>
                {
                    var z = factory.GetClient<IFuzzyMatcher>(bucket);

                    return z.FuzzyQuery(data.Select(x=>new FuzzyCheck{Phrase=x}));

                }));
            }
            Task.WaitAll(tasks.ToArray());

            sw.Stop();

            Console.WriteLine($"Elapsed time - {sw.ElapsedMilliseconds}");

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
            }
        }
    }
}
