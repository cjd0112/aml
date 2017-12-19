using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using As.Logger;
using Microsoft.Data.Sqlite;

namespace As.GraphDB.Sql
{
    public class SqlTableComplexLinkages<T,Y> : SqlTableBase where T : struct, IConvertible where Y : struct,IConvertible
    {
        public enum SqlLinkageField
        {
            FromType,
            ToType,
            ToId,
            FromId,
            RelationType
        }
        public SqlTableComplexLinkages()
        {
        }

        public int CreateManyToManyLinkagesTable(SqliteConnection conn, String tableName)
        {
            int foo = 0;
            foo = ExecuteCommandLog(conn, $@"create table {tableName} (FromType text, FromId text, ToType text, ToId text, RelationType text);
            create index {tableName}_FromType_idx on {tableName}(FromType);
            create index {tableName}_ToType_idx on {tableName}(ToType);
            create index {tableName}_FromId_idx on {tableName}(FromId);
            create index {tableName}_ToId_idx on {tableName}(ToId);
            create index {tableName}_RelationType_idx on {tableName}(RelationType);");

            return foo;
        }

        public int InsertOrReplaceLinkageRows(SqliteConnection connection, String tableName, IEnumerable<SqlComplexLinkage<T,Y>> linkages)
        {
            int cnt = 0;

            using (var txn = connection.BeginTransaction())
            {
                String insertSql =
                    $"insert or replace into {tableName} (FromType,FromId,ToType,ToId,RelationType) values ($fromType,$fromId,$toType,$toId,$relationType);";

                foreach (var c in linkages)
                {
                    using (var insertCommand = connection.CreateCommand())
                    {
                        insertCommand.CommandText = insertSql;

                        insertCommand.Parameters.AddWithValue("$fromType", c.FromType);
                        insertCommand.Parameters.AddWithValue("$fromId", c.FromId);
                        insertCommand.Parameters.AddWithValue("$toType", c.ToType);
                        insertCommand.Parameters.AddWithValue("$toId", c.ToId);
                        insertCommand.Parameters.AddWithValue("$relationType", c.RelationType);
                        try
                        {
                            insertCommand.ExecuteNonQuery();

                            cnt++;
                        }
                        catch (Exception e)
                        {
                            L.Trace(e.Message);
                        }
                    }
                    txn.Commit();
                }
                return cnt;
            }
        }

        public IEnumerable<SqlComplexLinkage<T,Y>> QueryLinkageRows(SqliteConnection connection, String tableName,IEnumerable<(SqlLinkageField,Object)> query)
        {
            StringBuilder queryCommand = new StringBuilder($"select FromType,FromId,ToType,ToId, RelationType from {tableName} where ");

            foreach (var c in query)
            {
                queryCommand.Append($" {c.Item1} = '{c.Item2.ToString()}' ");
                queryCommand.Append(" and ");
                queryCommand.Remove(queryCommand.Length - " and ".Length, queryCommand.Length - " and ".Length);

                queryCommand.Append(";");
            }

            using (var queryCmd = connection.CreateCommand())
            {
                queryCmd.CommandText = queryCommand.ToString();
                using (var reader = queryCmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        yield break;
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            yield return new SqlComplexLinkage<T,Y>(reader);
                        }
                    }
                }
            }
        }
    }
}
