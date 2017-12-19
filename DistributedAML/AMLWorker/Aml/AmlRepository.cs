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

        private SqlTableWithId sqlTableWithId;
        
        public AmlRepository(IServiceServer server) : base(server)
        {
            sqlTableWithId = new SqlTableWithId();

            connectionString = sqlTableWithId.GetConnectionString(
                (string) server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                if (!sqlTableWithId.TableExists(connection, partySql))
                {
                    sqlTableWithId.CreateTable(connection,partySql);
                }

                if (!sqlTableWithId.TableExists(connection, accountSql))
                {
                    sqlTableWithId.CreateTable(connection,accountSql);
                }

                if (!sqlTableWithId.TableExists(connection, "AccountParty"))
                {
                    new SqlTableSimpleLinkages().CreateManyToManyLinkagesTableWithForeignKeyConstraint(connection, "AccountParty",
                        "AccountId", "PartyId", "Accounts", "Id");
                }

                if (!sqlTableWithId.TableExists(connection, "PartyAccount"))
                {
                    new SqlTableSimpleLinkages().CreateManyToManyLinkagesTable(connection, "PartyAccount", "PartyId", "AccountId");
                }

                if (!sqlTableWithId.TableExists(connection, transactionSql))
                {
                    sqlTableWithId.CreateTable(connection,transactionSql);
                }
            }
        }



        public override int StoreParties(IEnumerable<Party> parties)
        {
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                return sqlTableWithId.InsertOrReplace(connection, partySql, parties);
            }
        }

        public override int StoreAccounts(IEnumerable<Account> accounts)
        {
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                return sqlTableWithId.InsertOrReplace(connection, accountSql,accounts);
            }
        }

        public override int StoreLinkages(IEnumerable<AccountToParty> mappings, LinkageDirection dir)
        {
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                if (dir == LinkageDirection.AccountToParty)
                {
                    return new SqlTableSimpleLinkages().InsertOrUpdateLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        mappings.Cast<Object>(),
                        (x) =>
                        {
                            var t = (AccountToParty) x;
                            return (t.AccountId, t.PartyId);

                        });
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    return new SqlTableSimpleLinkages().InsertOrUpdateLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
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
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                if (dir == LinkageDirection.AccountToParty)
                {
                    foreach (var c in new SqlTableSimpleLinkages().QueryLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        source.Cast<Object>(), x => ((Identifier)x).Id ))
                    {
                        ret.Add(new AccountToParty {AccountId = c.Item1, PartyId = c.Item2});
                    }
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    foreach (var c in new SqlTableSimpleLinkages().QueryLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
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
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                foreach (var c in sqlTableWithId.QueryId(connection, accountSql,account.Select(x=>x.Id)))
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
            using (var connection = sqlTableWithId.NewConnection(connectionString))
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
            using (var connection = sqlTableWithId.NewConnection(connectionString))
            {
                return sqlTableWithId.InsertOrReplace(connection, transactionSql, txns);
            }
        }
    }
}