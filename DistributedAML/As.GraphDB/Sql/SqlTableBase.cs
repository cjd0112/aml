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
        public String GetConnectionString(String dataDirectory, int bucket, String dbFile)
        {
            if (Directory.Exists(dataDirectory) == false)
                Directory.CreateDirectory(dataDirectory);
            return $"{dataDirectory}/{dbFile}_{bucket}.mdb";
        }


        public SqliteConnection NewConnection(string connectionString)
        {
            var c = new SqliteConnection(connectionString);

            c.Open();
            return c;
        }

        public bool TableExists(SqliteConnection conn, String tableName)
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



        public  bool IndexExists(SqliteConnection conn, String tableName, string columnName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"pragma index_info({tableName + "_" + columnName});";
                var rdr = cmd.ExecuteReader();
                return rdr.HasRows;
            }
        }

        public int CreateIndex(SqliteConnection conn, String tableName, String columnName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"create index {tableName + "_" + columnName} on {tableName}({columnName})";
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
