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
    public class LoadFromCSV: AmlCommand
    {
        enum DataType
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

        public override void Run()
        {
            L.Trace($"LoadFromCSV started @ {DateTime.Now}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            init.ValidateServiceBucketsAreConsistent(typeof(IPartyStore));

            var buckets = factory.GetClientBuckets<IPartyStore>().Count();

            List<Task> tasks = new List<Task>();

            if (dataType == DataType.Party)
            {
                Multiplexer.FromCsv<Party>(GetDataFile(), buckets, (x) =>
                {
                    x.Type = partyType;
                    return x.Id;
                },
                x =>
                {
                    tasks.Add(new TaskManageException("StoreParties",() => factory.GetClient<IPartyStore>(x.Item1).StoreParties(x.Item2)));
                });
            }
            else if (dataType == DataType.Account)
            {
                Multiplexer.FromCsv<Account>(GetDataFile(), buckets, (x) => x.Id,
                    x =>
                    {
                        tasks.Add(new TaskManageException("StoreAccounts",() => factory.GetClient<IPartyStore>(x.Item1).StoreAccounts(x.Item2)));
                    });
            }
            else if (dataType == DataType.AccountToParty)
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
                        tasks.Add(new TaskManageException("StoreLinkages",() => factory.GetClient<IPartyStore>(x.Item1).StoreLinkages(x.Item2,linkageDirection)));
                    });
            }

            else
            {
                throw new Exception($"Unexpected data type - {dataType}");
            }
            tasks.Do(x => x.Start());
            Task.WaitAll(tasks.ToArray());
            sw.Stop();
            L.Trace($"Elapsed time for operation - {sw.ElapsedMilliseconds}");
        }

    }
}
