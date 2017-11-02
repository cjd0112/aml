using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        string TransactionMapperCreate = "create table TransactionStore (id string primary key, blob transaction);";
        private string connectionString;

        public TransactionStore(IServiceServer server) : base(server)
        {
            L.Trace(
                $"Opened server with bucket {server.BucketId} and data dir - {server.GetConfigProperty("DataDirectory", server.BucketId)}");

            connectionString = (string) server.GetConfigProperty("DataDirectory", server.BucketId) +
                               $"/AmlWorker_{server.BucketId}";
            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = newConnection())
            {
                connection.Open();

                if (!SqlHelper.TableExists(connection, "TransactionStore"))
                {
                    SqlHelper.ExecuteCommandLog(connection, TransactionMapperCreate);

                }
            }
        }

        SqliteConnection newConnection()
        {
            return new SqliteConnection(
                "" + new SqliteConnectionStringBuilder {DataSource = $"{connectionString}"});
        }

        public override int StoreTransactions(List<Transaction> transactions)
        {
            int cnt = 0;

            var s = new Stopwatch();
            s.Start();
            L.Trace(
                $"TransactionStore- {this.server.BucketId} - hit add entry with {transactions.Count} at {DateTime.Now}");
            using (var connection = newConnection())
            {
                connection.Open();
                var txn = connection.BeginTransaction();

                foreach (var c in transactions)
                {
                    using (var existsCmd = connection.CreateCommand())
                    {
                        existsCmd.CommandText = "select rowid from TransactionStore where Id=($id)";

                        existsCmd.Parameters.AddWithValue("$id", c.Id);
                        var exists = existsCmd.ExecuteReader();
                        if (exists.HasRows)
                        {
                            using (var insert1Cmd = connection.CreateCommand())
                            {
                                insert1Cmd.Transaction = txn;
                                insert1Cmd.CommandText =
                                    "update TransactionStore set Data='$data' where Id=($id);";

                                exists.Read();
                                var phraseid = exists.GetInt32(0);

                                insert1Cmd.Parameters.AddWithValue("id", c.Id);
                                insert1Cmd.Parameters.AddWithValue("$data", c.ToByteArray());

                                try
                                {
                                    var res = insert1Cmd.ExecuteNonQuery();
                                    cnt++;
                                }
                                catch (Exception e)
                                {
                                    L.Trace(e.Message);
                                }
                            }
                        }
                        else
                        {
                            using (var insert2Cmd = connection.CreateCommand())
                            {
                                insert2Cmd.Transaction = txn;
                                insert2Cmd.CommandText =
                                    "insert into TransactionStore (Id,Data) values ($id,$data);";

                                insert2Cmd.Parameters.AddWithValue("$id", c.Id);
                                insert2Cmd.Parameters.AddWithValue("$data", c.ToByteArray());

                                try
                                {
                                    insert2Cmd.ExecuteNonQuery();
                                    cnt++;
                                }
                                catch (Exception e)
                                {
                                    L.Trace(e.Message);
                                }
                            }
                        }
                    }
                }
            }
            return cnt;
        }
    }
}