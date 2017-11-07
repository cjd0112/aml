using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Logger;
using Microsoft.Data.Sqlite;

namespace Shared
{
    public static class SqlHelper
    {
        public static String GetConnectionString(String dataDirectory, int bucket, String dbFile)
        {
            return $"{dataDirectory}/{dbFile}_{bucket}.mdb";

        }

        public static SqliteConnection NewConnection(string connectionString)
        {
            return new SqliteConnection(
                "" + new SqliteConnectionStringBuilder { DataSource = $"{connectionString}" });
        }

        public static bool TableExists(SqliteConnection conn, String tableName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}';";
                using (var rdr = cmd.ExecuteReader())
                {
                    return rdr.HasRows;
                }
            }
        }
       

        public static int ExecuteCommandLog(SqliteConnection conn, String cmdText)
        {
            L.Trace($"Executing - {cmdText}");
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                return cmd.ExecuteNonQuery();
            }
        }

        public static int CreateBlobTable(SqliteConnection conn, String tableName)
        {
            return ExecuteCommandLog(conn,$"create table {tableName} (matchkey string primary key, data blob);");
        }

        public static int InsertOrUpdateRows(SqliteConnection connection, String tableName, List<Object> objs,
            Func<Object, Tuple<string, byte[]>> GetMatchkeyAndBytes)
        {
            int cnt = 0;

            var txn = connection.BeginTransaction();

            String queryCommand = $"select rowid from {tableName} where matchkey=($matchkey)";
            String updateCommand = $"update {tableName} set data='$data' where matchkey=($matchkey);";
            String insertCommand = $"insert into {tableName} (matchkey,data) values ($matchkey,$data);";
            
            foreach (var c in objs)
            {
                using (var existsCmd = connection.CreateCommand())
                {
                    existsCmd.CommandText = queryCommand;

                    var q = GetMatchkeyAndBytes(c);

                    existsCmd.Parameters.AddWithValue("$matchkey", q.Item1);
                    var exists = existsCmd.ExecuteReader();
                    if (exists.HasRows)
                    {
                        using (var insert1Cmd = connection.CreateCommand())
                        {
                            insert1Cmd.Transaction = txn;
                            insert1Cmd.CommandText = updateCommand;

                            exists.Read();
                            var phraseid = exists.GetInt32(0);

                            insert1Cmd.Parameters.AddWithValue("$matchkey", q.Item1);
                            insert1Cmd.Parameters.AddWithValue("$data", q.Item2);

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
                            insert2Cmd.CommandText = insertCommand;

                            insert2Cmd.Parameters.AddWithValue("$matchkey", q.Item1);
                            insert2Cmd.Parameters.AddWithValue("$data", q.Item2);

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

                if (cnt % 100000 == 0)
                {
                    txn.Commit();
                    txn = connection.BeginTransaction();
                }
                
            }
            txn.Commit();
            return cnt;
        }

    }
}
