using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Comms;
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
            connectionString = SqlHelper.GetConnectionString((string)server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (!SqlHelper.TableExists(connection, "PartyStore"))
                {
                    SqlHelper.CreateBlobTable(connection, "PartyStore");

                }
            }
        }

      

        public override int StoreParties(List<Party> parties)
        {
            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlHelper.InsertOrUpdateRows(connection, "PartyStore", parties.Cast<Object>().ToList(),
                    (x) =>
                    {
                        var t = (Party)x;
                        return new Tuple<string, byte[]>(t.GetMatchKey(), t.ToByteArray());

                    });
            }
        }
    }
}