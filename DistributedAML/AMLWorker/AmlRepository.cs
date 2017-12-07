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
            connectionString = SqlConnectionHelper.GetConnectionString(
                (string) server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (!SqlConnectionHelper.TableExists(connection, "Parties"))
                {
                    SqlConnectionHelper.CreateBlobTable(connection, "Parties");
                }

                if (!SqlConnectionHelper.TableExists(connection, "Accounts"))
                {
                    SqlConnectionHelper.CreateStandardTableWithIdPrimaryKey(connection, "Accounts", typeof(Account),
                        x => TypeCheck.IsScalar(x.PropertyType));
                   // SqlConnectionHelper.CreateBlobTable(connection, "Accounts");
                }


                if (!SqlConnectionHelper.TableExists(connection, "AccountParty"))
                {
                    SqlConnectionHelper.CreateManyToManyLinkagesTableWithForeignKeyConstraint(connection, "AccountParty",
                        "AccountId", "PartyId", "Accounts", "Id");
                }

                if (!SqlConnectionHelper.TableExists(connection, "PartyAccount"))
                {
                    SqlConnectionHelper.CreateManyToManyLinkagesTable(connection, "PartyAccount", "PartyId", "AccountId");
                }

                if (!SqlConnectionHelper.TableExists(connection, "Transactions"))
                {
                    SqlConnectionHelper.CreateBlobTable(connection, "Transactions");
                }
            }
        }



        public override int StoreParties(IEnumerable<Party> parties)
        {
            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlConnectionHelper.InsertOrUpdateBlobRows(connection, "Parties", parties.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Party) x;
                        return (t.Id, t.ToByteArray());

                    });
            }
        }

        public override int StoreAccounts(IEnumerable<Account> accounts)
        {
            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlConnectionHelper.InsertRows(connection, "Accounts", typeof(Account),
                    n => TypeCheck.IsScalar(n.PropertyType), accounts);

                /*
                return SqlConnectionHelper.InsertOrUpdateBlobRows(connection, "Accounts", accounts.Cast<Object>(),
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
            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (dir == LinkageDirection.AccountToParty)
                {
                    return SqlConnectionHelper.InsertOrUpdateLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        mappings.Cast<Object>(),
                        (x) =>
                        {
                            var t = (AccountToParty) x;
                            return (t.AccountId, t.PartyId);

                        });
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    return SqlConnectionHelper.InsertOrUpdateLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
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
            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (dir == LinkageDirection.AccountToParty)
                {
                    foreach (var c in SqlConnectionHelper.QueryLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        source.Cast<Object>(), x => ((Identifier)x).Id ))
                    {
                        ret.Add(new AccountToParty {AccountId = c.Item1, PartyId = c.Item2});
                    }
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    foreach (var c in SqlConnectionHelper.QueryLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
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
            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
            {
                connection.Open();
                foreach (var c in SqlConnectionHelper.QueryId(connection, "Accounts", account.Cast<Object>(), x => ((Identifier)x).Id))
                {
                    ret.Add(new YesNo{Val=c.Item2});
                }
            }
            return ret;
        }


      

        public override GraphResponse RunQuery(GraphQuery query)
        {
            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
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
            using (var connection = SqlConnectionHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlConnectionHelper.InsertOrUpdateBlobRows(connection, "Transactions", txns.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Transaction)x;
                        return (t.Id, t.ToByteArray());

                    });
            }
        }

      
    }
}