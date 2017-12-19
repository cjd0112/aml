using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime.Atn;
using As.Logger;
using Microsoft.Data.Sqlite;

namespace As.GraphDB.Sql
{
    public  class SqlTableWithId : SqlTableBase
    {
        public  int GetNextId(SqliteConnection conn,string tableName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"select max(rowid) from {tableName};";
                var z = cmd.ExecuteScalar();
                return (Convert.ToInt32(z));
            }
            
        } 


        public  bool TableExists<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands)
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


        public  int CreateTable<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands)
        {
            return ExecuteCommandLog(conn, propertiesAndCommands.CreateTableCommand());
        }

        public  void UpdateTableStructure<T>(SqliteConnection conn,SqlitePropertiesAndCommands<T> propertiesAndCommands)
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

    

        public  int AddColumn<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands, String columnName, ColumnType type)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = propertiesAndCommands.AddColumnCommand(columnName, type);
                return cmd.ExecuteNonQuery();
            }
        }

        public  int AddColumnValues<T>(SqliteConnection conn, SqlitePropertiesAndCommands<T> propertiesAndCommands, String columnName, ColumnType type,
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
   
        
        public  int InsertOrReplace<T>(SqliteConnection connection,   SqlitePropertiesAndCommands<T> propertiesAndCommands,IEnumerable<T> objs)
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

      

        public  IEnumerable<DataRecordHelper<T>> SelectData<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands,String whereClause,Range range,Sort sort)
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

        public  int Delete<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands,
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

        public  T SelectDataById<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands, String id)
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

        public  IEnumerable<(string, bool)> QueryId<T>(SqliteConnection connection, SqlitePropertiesAndCommands<T> propertiesAndCommands, IEnumerable<string> ids)
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
