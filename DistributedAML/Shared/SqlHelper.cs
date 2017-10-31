using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Logger;
using Microsoft.Data.Sqlite;

namespace Shared
{
    public class SqlHelper
    {
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

        static String textColumns(IEnumerable<String> g)
        {
            var z = g.Aggregate("", (x, y) => x + y + ",");
            return z.TrimEnd(',');
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

        public static int CreateTextSearchTable(SqliteConnection conn, string tableName, IEnumerable<String> columns)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"create virtual table {tableName} using fts4({textColumns(columns)});";
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
