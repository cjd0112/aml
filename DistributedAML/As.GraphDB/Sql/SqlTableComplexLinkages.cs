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

        public SqlTableComplexLinkages(String name) :base(name)
        {
            TableName = name;
        }

        public int CreateManyToManyLinkagesTable(SqliteConnection conn)
        {
            int foo = 0;
            foo = ExecuteCommandLog(conn, $@"create table {TableName} (FromType text, FromId text, ToType text, ToId text, RelationType text);
            create index {TableName}_FromType_idx on {TableName}(FromType);
            create index {TableName}_ToType_idx on {TableName}(ToType);
            create index {TableName}_FromId_idx on {TableName}(FromId);
            create index {TableName}_ToId_idx on {TableName}(ToId);
            create index {TableName}_RelationType_idx on {TableName}(RelationType);");

            return foo;
        }

        public int InsertOrReplaceLinkageRows(SqliteConnection connection, IEnumerable<SqlComplexLinkage<T,Y>> linkages)
        {
            int cnt = 0;

            using (var txn = connection.BeginTransaction())
            {
                String insertSql =
                    $"insert or replace into {TableName} (FromType,FromId,ToType,ToId,RelationType) values ($fromType,$fromId,$toType,$toId,$relationType);";

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

        public IEnumerable<SqlComplexLinkage<T,Y>> QueryLinkageRows(SqliteConnection connection, IEnumerable<(SqlLinkageField,Object)> query)
        {
            StringBuilder queryCommand = new StringBuilder($"select FromType,FromId,ToType,ToId, RelationType from {TableName} where ");

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
