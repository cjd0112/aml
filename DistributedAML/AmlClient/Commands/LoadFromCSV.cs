using System;
using System.Collections;
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
    public class LoadFromCSV: AmlCommand
    {
        public enum DataType
        {
            Transaction,
            Account,
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

            if (dataType == DataType.Party || dataType == DataType.Account || dataType == DataType.AccountToParty)
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
                case DataType.Account:
                case DataType.AccountToParty:
                    ret = $"{reg.DataDirectory}/input/{partyType}{dataType}.csv";
                    break;
                case DataType.Transaction:
                    ret = $"{reg.DataDirectory}/input/Transactions.csv";
                    break;
                default:
                    throw new Exception($"Unexpected dataType - {dataType}");

            }
            return ret;
        }

        class TaskManageException: Task
        {
            public TaskManageException(String actionName,Action a) : base(() =>
            {
                try
                {
                    L.Trace($"Running {actionName}");
                    a();
                }
                catch (Exception e)
                {
                    L.Trace($"An exception encountered running {actionName} ...");
                    L.Trace(e.Message);
                }
            })
            {
                
            }
        }

        class TaskManageException2<T> : Task<T>
        {
            public List<Party> parties;

            public TaskManageException2(String actionName, List<Party> parties, Func<T> a) : base(() =>
            {
                try
                {
                    L.Trace($"Running {actionName}");
                    return a();
                }
                catch (Exception e)
                {
                    L.Trace($"An exception encountered running {actionName} ...");
                    L.Trace(e.Message);
                    return default(T);
                }
            })
            {
                this.parties = parties;
            }
        }

        public override void Run()
        {
            L.Trace($"LoadFromCSV started @ {DateTime.Now}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            init.ValidateServiceBucketsAreConsistent(typeof(IPartyStore));

            var buckets = factory.GetClientBuckets<IPartyStore>().Count();

            if (dataType == DataType.Party)
            {
                List<Task<List<AccountToParty>>> tasks = new List<Task<List<AccountToParty>>>();
                Multiplexer.FromCsv<Party>(GetDataFile(), buckets, (x) =>
                {
                    x.Type = partyType;
                    return x.Id;
                },
                x =>
                {
                    tasks.Add(
                        new TaskManageException2<List<AccountToParty>>("GetLinkages",x.Item2, () => 
                            factory.GetClient<IPartyStore>(x.Item1)
                              .GetLinkages(x.Item2.Select(j => j.Id).ToList(), LinkageDirection.PartyToAccount)));
                });

                tasks.Do(x=>x.Start());

                Task.WaitAll(tasks.ToArray());

                Multiplexer mp = new Multiplexer(buckets);

                foreach (TaskManageException2<List<AccountToParty>> z in tasks)
                {
                    mp.AddList(z.parties, (x) => 
                    {
                        z.Result.BinarySearch(new AccountToParty{PartyId = x})
                        
                    });
                }





            }
            else if (dataType == DataType.Account || dataType == DataType.AccountToParty)
            {
                List<Task> tasks = new List<Task>();

                if (dataType == DataType.Account)
                {
                    Multiplexer.FromCsv<Account>(GetDataFile(), buckets, (x) => x.Id,
                        x =>
                        {
                            tasks.Add(new TaskManageException("StoreAccounts",
                                () => factory.GetClient<IPartyStore>(x.Item1).StoreAccounts(x.Item2)));
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
                            tasks.Add(new TaskManageException("StoreLinkages", () => factory.GetClient<IPartyStore>(x.Item1).StoreLinkages(x.Item2, linkageDirection)));
                        });
                }

                tasks.Do((x)=>x.Start());
                Task.WaitAll(tasks.ToArray());

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
