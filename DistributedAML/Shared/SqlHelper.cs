using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Logger;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Shared
{
    public static class SqlHelper
    {
        public static String GetConnectionString(String dataDirectory, int bucket, String dbFile)
        {
            if (Directory.Exists(dataDirectory) == false)
                Directory.CreateDirectory(dataDirectory);
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
            return ExecuteCommandLog(conn,$"create table {tableName} (id string primary key, data blob);");
        }

        public static int CreateManyToManyLinkagesTable(SqliteConnection conn, String tableName,String link1,String link2)
        {
            int foo = 0;
            foo = ExecuteCommandLog(conn, $@"create table {tableName} ({link1} string, {link2} string);create index {link1}_idx on {tableName}({link1});");
            return foo; //ExecuteCommandLog(conn,$"create index {link1}_idx on {tableName}({link1});");
        }

        public static int InsertOrUpdateBlobRows(SqliteConnection connection, String tableName, List<Object> objs,
            Func<Object, (string, byte[])> GetIdAndBytes)
        {
            L.Trace($"Starting insert or update blob rows - {objs.Count} objects on {tableName}");
            int cnt = 0;

            var txn = connection.BeginTransaction();

            String queryCommand = $"select rowid from {tableName} where id=($id)";
            String updateCommand = $"update {tableName} set data='$data' where id=($id);";
            String insertCommand = $"insert into {tableName} (id,data) values ($id,$data);";
            
            foreach (var c in objs)
            {
                using (var existsCmd = connection.CreateCommand())
                {
                    existsCmd.CommandText = queryCommand;

                    var q = GetIdAndBytes(c);

                    existsCmd.Parameters.AddWithValue("$id", q.Item1);
                    var exists = existsCmd.ExecuteReader();
                    if (exists.HasRows)
                    {
                        using (var insert1Cmd = connection.CreateCommand())
                        {
                            insert1Cmd.Transaction = txn;
                            insert1Cmd.CommandText = updateCommand;

                            exists.Read();
                            var phraseid = exists.GetInt32(0);

                            insert1Cmd.Parameters.AddWithValue("$id", q.Item1);
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

                            insert2Cmd.Parameters.AddWithValue("$id", q.Item1);
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
                    L.Trace($"{cnt} objects committed on {tableName}");

                    txn.Commit();
                    txn = connection.BeginTransaction();
                }
                
            }
            txn.Commit();
            L.Trace($"{cnt} objects finished on {tableName}");
            return cnt;
        }

        public static int InsertOrUpdateLinkageRows(SqliteConnection connection, String tableName, String column1,String column2, List<Object> objs,Func<Object, (string, string)> GetMapping)
        {
            int cnt = 0;

            var txn = connection.BeginTransaction();

            String queryCommand = $"select rowid from {tableName} where {column1}=($id) and {column2}=($id2)";
            String insertCommand = $"insert into {tableName} ({column1},{column2}) values ($id,$id2);";

            foreach (var c in objs)
            {
                using (var existsCmd = connection.CreateCommand())
                {
                    existsCmd.CommandText = queryCommand;

                    var q = GetMapping(c);

                    existsCmd.Parameters.AddWithValue("$id", q.Item1);
                    existsCmd.Parameters.AddWithValue("$id2", q.Item2);
                    var exists = existsCmd.ExecuteReader();
                    if (!exists.HasRows)
                    {
                        using (var insertCmd = connection.CreateCommand())
                        {
                            insertCmd.Transaction = txn;
                            insertCmd.CommandText = insertCommand;

                            insertCmd.Parameters.AddWithValue("$id", q.Item1);
                            insertCmd.Parameters.AddWithValue("$id2", q.Item2);

                            try
                            {
                                insertCmd.ExecuteNonQuery();
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
