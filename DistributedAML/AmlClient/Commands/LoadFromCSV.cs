using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmlClient.AS.Application;
using AmlClient.Tasks;
using AmlClient.Utilities;
using Comms;
using CsvHelper;
using Logger;
using Shared;

namespace AmlClient.Commands
{
    public class LoadFromCSV: AmlCommand
    {
        public enum DataType
        {
            TransactionRoles,
            Transactions,
            Accounts,
            AccountToParty,
            Party
        }

        private ClientFactory factory;
        private Initialize init;
        private MyRegistry reg;
        private DataType dataType;
        private Party.Types.PartyType partyType;
        private LinkageDirection linkageDirection;
        public LoadFromCSV(Initialize init, ClientFactory factory, MyRegistry reg)
        {
            this.factory = factory;
            this.init = init;
            this.reg = reg;


            dataType = EnumHelper.Parse<DataType>(Helper.Prompt($"Enter type of data - {EnumHelper.ListValues(typeof(DataType))}"));

            if (dataType == DataType.Party)
            {
                partyType = EnumHelper.EnumPrompt<Party.Types.PartyType>();
            }

            if (dataType == DataType.AccountToParty)
            {
                linkageDirection = EnumHelper.EnumPrompt<LinkageDirection>();
            }
        }

        string GetDataFile()
        {
            string ret = "";
            switch (dataType)
            {
                case DataType.Party:
                    ret = $"{reg.DataDirectory}/input/{partyType}{dataType}.csv";
                    break;
                case DataType.Accounts:
                case DataType.AccountToParty:
                case DataType.Transactions:
                case DataType.TransactionRoles:
                    ret = $"{reg.DataDirectory}/input/{dataType}.csv";
                    break;
                default:
                    throw new Exception($"Unexpected dataType - {dataType}");

            }
            return ret;
        }

   
        public override void Run()
        {
            L.Trace($"LoadFromCSV started @ {DateTime.Now}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            init.ValidateServiceBucketsAreConsistent(typeof(IAmlRepository));

            var buckets = factory.GetClientBuckets<IAmlRepository>().Count();

            if (dataType == DataType.Party)
            {
                List<AmlClientTask<IEnumerable<AccountToParty>>> tasks = new List<AmlClientTask<IEnumerable<AccountToParty>>>();
                Multiplexer.FromCsv<Party>(GetDataFile(), buckets, (x) =>
                {
                    x.Type = partyType;
                    return x.Id;
                },
                x =>
                {
                    tasks.Add(
                        new AmlClientTask<IEnumerable<AccountToParty>>("GetLinkages", x.Item1, () => 
                            factory.GetClient<IAmlRepository>(x.Item1)
                              .GetLinkages(x.Item2.Select(j => new Identifier{Id=j.Id}).ToList(), LinkageDirection.PartyToAccount),x.Item2));
                });

                tasks.Do(x=>x.Start());

                Task.WaitAll(tasks.ToArray());

                List<Task<int>> partyTasks = new List<Task<int>>();

                Multiplexer mp = new Multiplexer(buckets);

                var comparer = new AccountToPartyComparer(LinkageDirection.PartyToAccount);

                foreach (var g in tasks)
                {
                    var parties = g.State as IEnumerable<Party>;
                    var linkages = g.Result.ToList();

                    linkages.Sort(comparer);

                    mp.AddList<Party>(parties.ToList(), (party) =>
                    {
                        var ret = linkages.BinarySearchMultiple(new AccountToParty {PartyId = party.Id}, comparer)
                            .Select(n => n.AccountId).ToArray();

                        if(ret.Length == 0)
                            throw new Exception($"Party id - {party.Id} not found in list");

                        return ret;
                    });


                    foreach (var b in mp.GetBuckets<Party>())
                    {
                        partyTasks.Add(new AmlClientTask<int>("StoreParties",b.Item1,()=>
                            factory.GetClient<IAmlRepository>(b.Item1)
                            .StoreParties(b.Item2)));
                    }
                }

                partyTasks.Do(x=>x.Start());
                Task.WaitAll(partyTasks.ToArray());


            }
            else if (dataType == DataType.Accounts || dataType == DataType.AccountToParty)
            {
                List<Task<int>> tasks = new List<Task<int>>();

                if (dataType == DataType.Accounts)
                {
                    Multiplexer.FromCsv<Account>(GetDataFile(), buckets, (x) => x.Id,
                        x =>
                        {
                            tasks.Add(new AmlClientTask<int>("StoreAccounts",x.Item1,()=>factory.GetClient<IAmlRepository>(x.Item1).StoreAccounts(x.Item2)));
                        });
                }
                else
                {
                    Multiplexer.FromCsv<AccountToParty>(GetDataFile(), buckets, (x) =>
                        {
                            if (linkageDirection == LinkageDirection.AccountToParty)
                                return x.AccountId;
                            else
                                return x.PartyId;
                        },
                        x =>
                        {
                            tasks.Add(new AmlClientTask<int>("StoreLinkages",x.Item1, () => factory.GetClient<IAmlRepository>(x.Item1).StoreLinkages(x.Item2, linkageDirection)));
                        });
                }

                tasks.Do((x)=>x.Start());
                Task.WaitAll(tasks.ToArray());

            }
            else if (dataType == DataType.Transactions)
            {
                List<Task<int>> tasks = new List<Task<int>>();
                Multiplexer.FromCsv<Transaction>(GetDataFile(), buckets, (x) => x.Id,
                    x =>
                    {
                        tasks.Add(new AmlClientTask<int>("StoreTransactions", x.Item1, () => factory.GetClient<IAmlRepository>(x.Item1).StoreTransactions(x.Item2)));
                    });

                tasks.Do((x) => x.Start());
                Task.WaitAll(tasks.ToArray());
            }
            else if (dataType == DataType.TransactionRoles)
            {
            }
            else
            {
                throw new Exception($"Unexpected data type - {dataType}");
            }
            sw.Stop();
            L.Trace($"Elapsed time for operation - {sw.ElapsedMilliseconds}");
        }

    }
}
