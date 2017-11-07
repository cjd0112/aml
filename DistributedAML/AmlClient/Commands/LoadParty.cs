using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmlClient.AS.Application;
using Comms;
using CsvHelper;
using Logger;
using Shared;

namespace AmlClient.Commands
{
    public class LoadParty : AmlCommand
    {
        private ClientFactory factory;
        private Initialize init;
        private MyRegistry reg;
        private String DataFile;
        private Party.Types.PartyType partyType;
        public LoadParty(Initialize init, ClientFactory factory, MyRegistry reg)
        {
            this.factory = factory;
            this.init = init;
            this.reg = reg;
            Console.WriteLine($"Enter file name from '{reg.DataDirectory}\\input' or 'y' to use default ('Retail-Large.csv')");
            var s = Console.ReadLine().ToLower();
            if (s == "y")
                DataFile = $"{reg.DataDirectory}\\input\\Retail-Large.csv";
            else
            {
                DataFile = $"{reg.DataDirectory}\\input\\s";
            }

            Console.WriteLine($"Enter party type - one of {Enum.GetNames(typeof(Party.Types.PartyType)).Aggregate("",(x,y)=>x+y+",")}");

            var partyTypeStr = Console.ReadLine();

            partyType = Enum.Parse<Party.Types.PartyType>(partyTypeStr, true);

            if (File.Exists(DataFile) == false)
                throw new Exception($"File does not exist - {DataFile}");
        }
        public override void Run()
        {
            L.Trace($"Party run started @ {DateTime.Now}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            init.ValidateServiceBucketsAreConsistent(typeof(IPartyStore));

            var maxBuckets = factory.GetClientBuckets<IPartyStore>().Max();

            var multiplexer = new Multiplexer<Party>(maxBuckets + 1 /*number of buckets*/);

            CsvReader rdr = new CsvReader(new StreamReader(DataFile));

            rdr.Configuration.HeaderValidated = null;

            rdr.Configuration.MissingFieldFound = null;

            var records = rdr.GetRecords<Party>();
            records.Take(100000)
                .Do(x =>
                {
                    x.Type = partyType;
                    multiplexer.Add(x.GetMatchKey(), x);
                });

            List<Task> tasks = new List<Task>();
            multiplexer.GetBuckets().Do(x =>
            {
                tasks.Add(new Task(() => factory.GetClient<IPartyStore>(x.Item1).StoreParties(x.Item2)));
                tasks.Last().Start();
            });

            Task.WaitAll(tasks.ToArray());
            sw.Stop();
            L.Trace($"Elapsed time for operation - {sw.ElapsedMilliseconds}");
        }

    }
}
