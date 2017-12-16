using System;
using System.Collections.Generic;
using System.Linq;
using As.Comms;
using As.Comms.ClientServer;
using As.GraphDB;
using As.GraphDB.Sql;
using As.Logger;

namespace AMLWorker.Aml
{
    public class AmlRepository : AmlRepositoryServer
    {
        private string connectionString;
        
        private SqlitePropertiesAndCommands<Party> partySql = new SqlitePropertiesAndCommands<Party>("Parties");
        private SqlitePropertiesAndCommands<Account> accountSql = new SqlitePropertiesAndCommands<Account>("Accounts");
        private SqlitePropertiesAndCommands<Transaction> transactionSql = new SqlitePropertiesAndCommands<Transaction>("Transactions");

        
        public AmlRepository(IServiceServer server) : base(server)
        {

            connectionString = SqlTableHelper.GetConnectionString(
                (string) server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                if (!SqlTableHelper.TableExists(connection, partySql))
                {
                    SqlTableHelper.CreateTable(connection,partySql);
                }

                if (!SqlTableHelper.TableExists(connection, accountSql))
                {
                    SqlTableHelper.CreateTable(connection,accountSql);
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

                if (!SqlTableHelper.TableExists(connection, transactionSql))
                {
                    SqlTableHelper.CreateTable(connection,transactionSql);
                }
            }
        }



        public override int StoreParties(IEnumerable<Party> parties)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return SqlTableHelper.InsertOrReplace(connection, partySql, parties);
            }
        }

        public override int StoreAccounts(IEnumerable<Account> accounts)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return SqlTableHelper.InsertOrReplace(connection, accountSql,accounts);
            }
        }

        public override int StoreLinkages(IEnumerable<AccountToParty> mappings, LinkageDirection dir)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
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
                foreach (var c in SqlTableHelper.QueryId(connection, accountSql,account.Select(x=>x.Id)))
                {
                    ret.Add(new YesNo{Val=c.Item2});
                }
            }
            return ret;
        }
        
        
        class GQ : IGraphQuery
        {
            public string OperationName { get; set; }
            public string Query { get; set; }
            public string Variables { get; set; }
        }

        public override GraphResponse RunQuery(GraphQuery query)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return new GraphResponse
                {
                    Response = new AmlRepositoryGraphDb(connection,partySql,accountSql,transactionSql).Run(new GQ
                    {
                        OperationName = query.OperationName,
                        Query = query.Query,
                        Variables = query.Variables
                    },new ProtobufCustomizeSchema()).ToString()
                };
            }
        }

        public override int StoreTransactions(IEnumerable<Transaction> txns)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return SqlTableHelper.InsertOrReplace(connection, transactionSql, txns);
            }
        }
    }
}