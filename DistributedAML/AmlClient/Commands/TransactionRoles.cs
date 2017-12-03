using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AmlClient.AS.Application;
using AmlClient.Tasks;
using AmlClient.Utilities;
using Comms;
using CsvHelper;
using Logger;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Shared;

namespace AmlClient.Commands
{
    public class TransactionRoles: AmlCommand
    {
        public enum RoleCheck
        {
            Internal,
        }

        private ClientFactory factory;
        private Initialize init;
        private MyRegistry reg;
        private RoleCheck roleCheck;
        public TransactionRoles(Initialize init, ClientFactory factory, MyRegistry reg)
        {
            this.factory = factory;
            this.init = init;
            this.reg = reg;

            roleCheck = EnumHelper.EnumPrompt<RoleCheck>();
        }

        string GetDataFile()
        {
            string ret = "";
            switch (roleCheck)
            {
                case RoleCheck.Internal:
                    ret = $"{reg.DataDirectory}/input/TransactionRoles.csv";
                    break;
                default:
                    throw new Exception($"Unexpected roleCheck - {roleCheck}");

            }
            return ret;
        }

        string GetNotFoundDirectResults()
        {
            return $"{reg.DataDirectory}/input/TransactionRolesNotFoundDirect.csv";
        }

        string GetFoundDirectResults()
        {
            return $"{reg.DataDirectory}/input/TransactionRolesFoundDirect.csv";
        }



        public override void Run()
        {
            L.Trace($"TransactionRoles started @ {DateTime.Now}");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            init.ValidateServiceBucketsAreConsistent(typeof(IAmlRepository));

            var buckets = factory.GetClientBuckets<IAmlRepository>().Count();

            List<AmlClientTask<IEnumerable<YesNo>>> tasks = new List<AmlClientTask<IEnumerable<YesNo>>>();
            Multiplexer.FromCsv<TransactionRole>(GetDataFile(), buckets, (x) => x.SortCode + x.Account,
                x =>
                {
                    tasks.Add(
                        new AmlClientTask<IEnumerable<YesNo>>("AccountsExist", x.Item1, () =>
                        {
                            L.Trace(
                                $"Hit this bucket start - {x.Item1} - Thread - {Thread.CurrentThread.ManagedThreadId}");
                            return factory.GetClient<IAmlRepository>(x.Item1)
                                .AccountsExist(x.Item2.Select(y => new Identifier {Id = y.SortCode + y.Account}));
                        }, x.Item2));

                });

            tasks.Do(x => x.Start());

            Task.WaitAll(tasks.ToArray());

            using (CsvWriter foundDirectWriter = new CsvWriter(new StreamWriter(GetFoundDirectResults())))
            {
                foundDirectWriter.WriteHeader<TransactionRole>();
                foundDirectWriter.NextRecord();

                using (CsvWriter notFoundDirectWriter = new CsvWriter(new StreamWriter(GetNotFoundDirectResults())))
                {
                    notFoundDirectWriter.WriteHeader<TransactionRole>();
                    notFoundDirectWriter.NextRecord();

                    foreach (var c in tasks)
                    {
                        var z = c.State as IEnumerable<TransactionRole>;
                        var h = c.Result;
                        if (z.Count() != h.Count())
                            throw new Exception($"Counts did not match .... ");

                        int foundDirect = 0;
                        int notFoundDirect = 0;
                        foreach (var q in z.Zip(h, (x, y) => (x, y)))
                        {
                            q.x.IsFoundDirect = !q.y.Val;
                            if (q.x.IsFoundDirect)
                            {
                                q.x.DiscoveredAccountId = q.x.SortCode + q.x.Account;
                                foundDirect++;
                                foundDirectWriter.WriteRecord(q.x);
                                foundDirectWriter.NextRecord();
                            }
                            else
                            {
                                notFoundDirect++;
                                notFoundDirectWriter.WriteRecord(q.x);
                                notFoundDirectWriter.NextRecord();

                            }
                        }

                        L.Trace($"found-cnt: {foundDirect}");
                        L.Trace($"not-found-cnt: {notFoundDirect}");
                        L.Trace($"total - {foundDirect + notFoundDirect}");
                    }
                }
                sw.Stop();
                L.Trace($"Elapsed time for operation - {sw.ElapsedMilliseconds}");
            }
        }

    }
}
