using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime.Atn;
using As.Logger;
using Microsoft.Data.Sqlite;

namespace As.GraphDB.Sql
{
    public static class SqlTableHelper
    {
        public static String GetConnectionString(String dataDirectory, int bucket, String dbFile)
        {
            if (Directory.Exists(dataDirectory) == false)
                Directory.CreateDirectory(dataDirectory);
            return $"{dataDirectory}/{dbFile}_{bucket}.mdb";
        }

        public static int GetNextId(SqliteConnection conn,string tableName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"select max(rowid) from {tableName};";
                var z = cmd.ExecuteScalar();
                return (Convert.ToInt32(z));
            }
            
        } 

        public static SqliteConnection NewConnection(string connectionString)
        {
            var c = new SqliteConnection(connectionString);

            c.Open();
            return c;
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

        public static bool TableExists<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{propertiesAndCommands.tableName}';";
                using (var rdr = cmd.ExecuteReader())
                {
                    return rdr.HasRows;
                }
            }
        }


        public static int CreateTable<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands)
        {
            return ExecuteCommandLog(conn, propertiesAndCommands.CreateTableCommand());
        }

        public static void UpdateTableStructure<T>(SqliteConnection conn,SqlitePropertiesAndCommands<T> propertiesAndCommands)
        {
            List<(string,string)> columns = new List<(string, string)>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"PRAGMA table_info(\"{propertiesAndCommands.tableName}\");";
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var name = (string) rdr["name"];
                        var type = (string) rdr["type"];
                        columns.Add((name,type));
                    }
                    
                }
            }

            foreach (var c in propertiesAndCommands.SqlFields())
            {
                if (!columns.Contains((c.pi.Name, "text")))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        L.Trace($"Adding new column - {c.pi.Name} to table {propertiesAndCommands.tableName}");
                        cmd.CommandText = propertiesAndCommands.AddColumnCommand(c.pi.Name, ColumnType.String);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        L.Trace($"Updating value of {c.pi.Name} in table {propertiesAndCommands.tableName}");
                        string value = c.pi.PropertyType.IsEnum ? Enum.GetNames(c.pi.PropertyType)[0] : "";
                        cmd.CommandText = propertiesAndCommands.UpdateColumnValuesCommandStr(c.pi.Name, value);
                        cmd.ExecuteNonQuery();
                    }

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
        
        public static bool IndexExists(SqliteConnection conn, String tableName, string columnName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"pragma index_info({tableName + "_" + columnName});";
                var rdr = cmd.ExecuteReader();
                return rdr.HasRows;
            }
        }

        public static int CreateIndex(SqliteConnection conn, String tableName, String columnName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"create index {tableName + "_" + columnName} on {tableName}({columnName})";
                return cmd.ExecuteNonQuery();
            }
        }

        public static int AddColumn<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands, String columnName, ColumnType type)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = propertiesAndCommands.AddColumnCommand(columnName, type);
                return cmd.ExecuteNonQuery();
            }
        }

        public static int AddColumnValues<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands, String columnName, ColumnType type,
            IEnumerable<(string id, Object value)> values)
        {
            int cnt = 0;
            using (var txn = conn.BeginTransaction())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = propertiesAndCommands.AddColumnCommand(columnName, type);

                    foreach (var c in values)
                    {
                        cmd.Parameters.AddWithValue("$value", c.value);
                        cmd.Parameters.AddWithValue("$id", c.id);

                        cmd.ExecuteNonQuery();

                        cnt++;
                    }
                }

                txn.Commit();
            }
            return cnt;
        }
        public static int CreateManyToManyLinkagesTable(SqliteConnection conn, String tableName,String link1,String link2)
        {
            int foo = 0;
            foo = ExecuteCommandLog(conn, $@"create table {tableName} ({link1} text, {link2} text);create index {link1}_idx on {tableName}({link1});");
            return foo; 
        }

        public static int CreateManyToManyLinkagesTableWithForeignKeyConstraint(SqliteConnection conn, String tableName,String link1,String link2,String parentTable,String parentColumn)
        {
            int foo = 0;
            foo = ExecuteCommandLog(conn, $@"create table {tableName} ({link1} text references {parentTable}({parentColumn}), {link2} text);create index {link1}_idx on {tableName}({link1});");
            return foo; 
        }
        
        public static int InsertOrReplace<T>(SqliteConnection connection,   SqlitePropertiesAndCommands<T> propertiesAndCommands,IEnumerable<T> objs)
        {
            L.Trace($"Starting insert rows - {objs.Count()} objects on {propertiesAndCommands.tableName}");
            int cnt = 0;

            String insertCommand = propertiesAndCommands.InsertOrReplaceCommand();

            L.Trace(insertCommand);

            var txn = connection.BeginTransaction();
            {
                foreach (var c in objs)
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        string id = "";
                        cmd.CommandText = insertCommand;
                        foreach (var g in propertiesAndCommands.SqlFields())
                        {
                            if (g.pi.Name.ToLower() == "id")
                                id = (string)g.getter(c);

                            if (g.pi.PropertyType.IsEnum)
                            {
                                cmd.Parameters.AddWithValue(g.pi.Name, g.getter(c).ToString());
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(g.pi.Name, g.getter(c));
                            }
                        }

                        try
                        {
                            var res = cmd.ExecuteNonQuery();
                            cnt++;
                        }
                        catch (Exception e)
                        {
                            L.Trace(e.Message);
                        }

                        if (cnt % 100000 == 0)
                        {
                            L.Trace($"{cnt} objects committed on {propertiesAndCommands.tableName}");

                            txn.Commit();
                            txn = connection.BeginTransaction();
                        }
                    }
                }
            }
            txn.Commit();
            L.Trace($"{cnt} objects finished on {propertiesAndCommands.tableName}");
            return cnt;
        }

        public static int InsertOrUpdateLinkageRows(SqliteConnection connection, String tableName, String column1,String column2, IEnumerable<Object> objs,Func<Object, (string, string)> GetMapping)
        {
            int cnt = 0;

            var txn = connection.BeginTransaction();

            using (var pragmaCmd = connection.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                pragmaCmd.ExecuteNonQuery();
            }


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
                    L.Trace($"{cnt} objects committed on {tableName}");

                }

            }
            txn.Commit();
            return cnt;
        }

        public static IEnumerable<(string,string)> QueryLinkageRows(SqliteConnection connection, String tableName, String queryColumn, String retrievalColumn, IEnumerable<Object> objs, Func<Object, string> GetMapping)
        {
            var txn = connection.BeginTransaction();

            String queryCommand = $"select {retrievalColumn} from {tableName} where {queryColumn}=($id)";

            foreach (var c in objs)
            {
                using (var queryCmd = connection.CreateCommand())
                {
                    queryCmd.CommandText = queryCommand;

                    var q = GetMapping(c);

                    queryCmd.Parameters.AddWithValue("$id", q);
                    using (var query = queryCmd.ExecuteReader())
                    {
                        if (!query.HasRows)
                        {
                            yield return (q, "");
                        }
                        else
                        {
                            while (query.Read())
                            {
                                yield return (q, query.GetString(0));
                            }
                        }
                    }
                }
            }
            txn.Commit();
        }

        public static IEnumerable<DataRecordHelper<T>> SelectData<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands,String whereClause,Range range,Sort sort)
        {
            using (var txn = connection.BeginTransaction())
            {
                string selectCommand;
                if (String.IsNullOrEmpty(whereClause))
                    selectCommand = $"{propertiesAndCommands.SelectCommand()} where {propertiesAndCommands.RangeClause(range)} {propertiesAndCommands.SortClause(sort)};";
                else
                    selectCommand = $"{propertiesAndCommands.SelectCommand()} where {whereClause} and {propertiesAndCommands.RangeClause(range)} {propertiesAndCommands.SortClause(sort)};";

                using (var queryCmd = connection.CreateCommand())
                {
                    queryCmd.CommandText = selectCommand;

                    using (var data = queryCmd.ExecuteReader())
                    {
                        var p = new DataRecordHelper<T>(propertiesAndCommands,data);

                        while (data.Read())
                        {
                            yield return p.IncrementCount(); 
                        }
                    }

                }
                txn.Commit();
                
            }
        }

        public static int Delete<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands,
            String id)
        {
            using (var txn = connection.BeginTransaction())
            {
                using (var deleteCmd = connection.CreateCommand())
                {
                    deleteCmd.CommandText = propertiesAndCommands.DeleteCommand(id);
                    deleteCmd.ExecuteNonQuery();
                }
                txn.Commit();
            }
            return 1;

        }

        public static T SelectDataById<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands, String id)
        {
            using (var txn = connection.BeginTransaction())
            {
                string selectCommand = $"{propertiesAndCommands.SelectCommand()} where id=\"{id}\";";

                using (var queryCmd = connection.CreateCommand())
                {
                    queryCmd.CommandText = selectCommand;

                    using (var data = queryCmd.ExecuteReader())
                    {
                        var p = new DataRecordHelper<T>(propertiesAndCommands, data);

                        if (data.Read())
                        {
                            var z = new DataRecordHelper<T>(propertiesAndCommands, data);
                            return z.GetObject();
                        }
                    }

                }
                txn.Commit();
            }
            return default(T);
        }

        public static IEnumerable<(string, bool)> QueryId<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands, IEnumerable<string> ids)
        {
            using (var txn = connection.BeginTransaction())
            {
                String queryCommand = propertiesAndCommands.QueryIdCommand();

                foreach (var c in ids)
                {
                    using (var queryCmd = connection.CreateCommand())
                    {
                        queryCmd.CommandText = queryCommand;

                        queryCmd.Parameters.AddWithValue("$id", c);
                        int cnt = Convert.ToInt32(queryCmd.ExecuteScalar());
                        yield return (c, cnt > 0);
                    }
                }
                txn.Commit();
            }
        }
        
    }
}
