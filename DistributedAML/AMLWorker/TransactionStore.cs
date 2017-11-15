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
    public class TransactionStore : TransactionStoreServer
    {
        private string connectionString;

        public TransactionStore(IServiceServer server) : base(server)
        {
            L.Trace(
                $"Opened server with bucket {server.BucketId} and data dir - {server.GetConfigProperty("DataDirectory", server.BucketId)}");

            connectionString = SqlHelper.GetConnectionString((string)server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                if (!SqlHelper.TableExists(connection, "TransactionStore"))
                {
                    SqlHelper.CreateBlobTable(connection, "TransactionStore");

                }
            }
        }

        public override int StoreTransactions(IEnumerable<Transaction> transactions)
        {
            using (var connection = SqlHelper.NewConnection(connectionString))
            {
                connection.Open();

                return SqlHelper.InsertOrUpdateBlobRows(connection, "TransactionStore", transactions.Cast<Object>(),
                    (x) =>
                    {
                        var t = (Transaction) x;
                        return (t.Id, t.ToByteArray());

                    });
            }
        }
    }
}