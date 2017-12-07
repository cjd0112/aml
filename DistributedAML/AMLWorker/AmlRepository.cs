using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using AMLWorker.Sql;
using As.Comms;
using As.Comms.ClientServer;
using Fasterflect;
using Google.Protobuf;
using GraphQL;
using As.GraphQL.Interface;
using As.Logger;
using Microsoft.Data.Sqlite;
using As.Shared;

namespace AMLWorker
{
    public class AmlRepository : AmlRepositoryServer
    {
        private string connectionString;

        public AmlRepository(IServiceServer server) : base(server)
        {
            connectionString = SqlTableHelper.GetConnectionString(
                (string) server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (!SqlTableHelper.TableExists(connection, "Parties"))
                {
                    SqlTableHelper.CreateBlobTable(connection, "Parties");
                }

                if (!SqlTableHelper.TableExists(connection, "Accounts"))
                {
                    SqlTableHelper.CreateStandardTableWithIdPrimaryKey(connection, "Accounts", typeof(Account),
                        x => TypeCheck.IsScalar(x.PropertyType));
                   // SqlTableHelper.CreateBlobTable(connection, "Accounts");
                }


                if (!SqlTableHelper.TableExists(connection, "AccountParty"))
                {
                    SqlTableHelper.CreateManyToManyLinkagesTableWithForeignKeyConstraint(connection, "AccountParty",
                        "AccountId", "PartyId", "Accounts", "Id");
                }

                if (!SqlTableHelper.TableExists(connection, "PartyAccount"))
                {
                    SqlTableHelper.CreateManyToManyLinkagesTable(connection, "PartyAccount", "PartyId", "AccountId");
                }

                if (!SqlTableHelper.TableExists(connection, "Transactions"))
                {
                    SqlTableHelper.CreateBlobTable(connection, "Transactions");
                }
            }
        }



        public override int StoreParties(IEnumerable<Party> parties)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlTableHelper.InsertOrUpdateBlobRows(connection, "Parties", parties.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Party) x;
                        return (t.Id, t.ToByteArray());

                    });
            }
        }

        public override int StoreAccounts(IEnumerable<Account> accounts)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlTableHelper.InsertRows(connection, "Accounts", typeof(Account),
                    n => TypeCheck.IsScalar(n.PropertyType), accounts);

                /*
                return SqlTableHelper.InsertOrUpdateBlobRows(connection, "Accounts", accounts.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Account) x;
                        return (t.Id, t.ToByteArray());

                    });
                */
            }
        }

        public override int StoreLinkages(IEnumerable<AccountToParty> mappings, LinkageDirection dir)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (dir == LinkageDirection.AccountToParty)
                {
                    return SqlTableHelper.InsertOrUpdateLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        mappings.Cast<Object>(),
                        (x) =>
                        {
                            var t = (AccountToParty) x;
                            return (t.AccountId, t.PartyId);

                        });
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    return SqlTableHelper.InsertOrUpdateLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
                        mappings.Cast<Object>(),
                        (x) =>
                        {
                            var t = (AccountToParty) x;
                            return (t.PartyId, t.AccountId);

                        });

                }
                else
                {
                    throw new Exception($"Unexpected LinkageDirection - {dir}");
                }
            }
        }

        public override IEnumerable<AccountToParty> GetLinkages(IEnumerable<Identifier> source, LinkageDirection dir)
        {
            var ret = new List<AccountToParty>();
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (dir == LinkageDirection.AccountToParty)
                {
                    foreach (var c in SqlTableHelper.QueryLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        source.Cast<Object>(), x => ((Identifier)x).Id ))
                    {
                        ret.Add(new AccountToParty {AccountId = c.Item1, PartyId = c.Item2});
                    }
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    foreach (var c in SqlTableHelper.QueryLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
                        source.Cast<Object>(), x => ((Identifier)x).Id))
                    {
                        ret.Add(new AccountToParty {PartyId = c.Item1, AccountId = c.Item2});
                    }
                }
                else
                {
                    throw new Exception($"Unexpected LinkageDirection - {dir}");
                }
            }
            return ret;
        }

        public override IEnumerable<YesNo> AccountsExist(IEnumerable<Identifier> account)
        {
            var ret = new List<YesNo>();
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();
                foreach (var c in SqlTableHelper.QueryId(connection, "Accounts", account.Cast<Object>(), x => ((Identifier)x).Id))
                {
                    ret.Add(new YesNo{Val=c.Item2});
                }
            }
            return ret;
        }


      

        public override GraphResponse RunQuery(GraphQuery query)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();

                return new GraphResponse
                {
                    Response = new AmlRepositoryGraphDb(connection).Run(query.Query).ToString()
                };
            }
        }

        public override int StoreTransactions(IEnumerable<Transaction> txns)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlTableHelper.InsertOrUpdateBlobRows(connection, "Transactions", txns.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Transaction)x;
                        return (t.Id, t.ToByteArray());

                    });
            }
        }

      
    }
}