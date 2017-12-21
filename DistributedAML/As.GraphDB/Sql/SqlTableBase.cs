using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using As.Logger;
using Microsoft.Data.Sqlite;

namespace As.GraphDB.Sql
{
    public class SqlTableBase
    {
        public SqlTableBase(string tableName)
        {
            this.TableName = tableName;

        }
        public String TableName { get; set; }

    

        public bool TableExists(SqliteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{TableName}';";
                using (var rdr = cmd.ExecuteReader())
                {
                    return rdr.HasRows;
                }
            }
        }


        public bool TableExists(SqliteConnection conn,String tableName)
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

        public int ExecuteCommandLog(SqliteConnection conn, String cmdText)
        {
            L.Trace($"Executing - {cmdText}");
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                return cmd.ExecuteNonQuery();
            }
        }



        public  bool IndexExists(SqliteConnection conn, string columnName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"pragma index_info({TableName + "_" + columnName});";
                var rdr = cmd.ExecuteReader();
                return rdr.HasRows;
            }
        }

        public int CreateIndex(SqliteConnection conn, String columnName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"create index {TableName + "_" + columnName} on {TableName}({columnName})";
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
