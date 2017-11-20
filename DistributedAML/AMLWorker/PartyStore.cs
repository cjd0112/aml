using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Comms;
using Comms.ClientServer;
using Google.Protobuf;
using Logger;
using Microsoft.Data.Sqlite;
using Shared;

namespace AMLWorker
{
    public class PartyStore : PartyStoreServer
    {
        private string connectionString;

        public PartyStore(IServiceServer server) : base(server)
        {
            connectionString = SqlHelper.GetConnectionString(
                (string) server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (!SqlHelper.TableExists(connection, "Parties"))
                {
                    SqlHelper.CreateBlobTable(connection, "Parties");
                }

                if (!SqlHelper.TableExists(connection, "Accounts"))
                {
                    SqlHelper.CreateBlobTable(connection, "Accounts");
                }


                if (!SqlHelper.TableExists(connection, "AccountParty"))
                {
                    SqlHelper.CreateManyToManyLinkagesTableWithForeignKeyConstraint(connection, "AccountParty",
                        "AccountId", "PartyId", "Accounts", "Id");
                }

                if (!SqlHelper.TableExists(connection, "PartyAccount"))
                {
                    SqlHelper.CreateManyToManyLinkagesTable(connection, "PartyAccount", "PartyId", "AccountId");
                }
            }
        }



        public override int StoreParties(IEnumerable<Party> parties)
        {
            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlHelper.InsertOrUpdateBlobRows(connection, "Parties", parties.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Party) x;
                        return (t.Id, t.ToByteArray());

                    });
            }
        }

        public override int StoreAccounts(IEnumerable<Account> accounts)
        {
            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlHelper.InsertOrUpdateBlobRows(connection, "Accounts", accounts.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Account) x;
                        return (t.Id, t.ToByteArray());

                    });
            }
        }

        public override int StoreLinkages(IEnumerable<AccountToParty> mappings, LinkageDirection dir)
        {
            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (dir == LinkageDirection.AccountToParty)
                {
                    return SqlHelper.InsertOrUpdateLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        mappings.Cast<Object>(),
                        (x) =>
                        {
                            var t = (AccountToParty) x;
                            return (t.AccountId, t.PartyId);

                        });
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    return SqlHelper.InsertOrUpdateLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
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
            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (dir == LinkageDirection.AccountToParty)
                {
                    foreach (var c in SqlHelper.QueryLinkageRows(connection, "AccountParty", "AccountId", "PartyId",
                        source.Cast<Object>(), x => ((Identifier)x).Id ))
                    {
                        ret.Add(new AccountToParty {AccountId = c.Item1, PartyId = c.Item2});
                    }
                }
                else if (dir == LinkageDirection.PartyToAccount)
                {
                    foreach (var c in SqlHelper.QueryLinkageRows(connection, "PartyAccount", "PartyId", "AccountId",
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
    }
}