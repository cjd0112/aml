using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AMLWorker.Sql;
using As.Logger;
using Fasterflect;
using Microsoft.Data.Sqlite;

namespace AMLWorker
{
    public static class SqlHelper2
    {
        public enum ColumnType2
        {
            String,
            Numeric
        }

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
            return ExecuteCommandLog(conn,$"create table {tableName} (id text primary key, data blob);");
        }

        public static int CreateStandardTableWithIdPrimaryKey(SqliteConnection conn, String tableName,Type t,Predicate<PropertyInfo> pis)
        {
            var b = new StringBuilder();

            b.Append($"create table {tableName} (");
            foreach (var c in t.GetProperties())
            {
                if (pis(c))
                {
                    if (c.Name.ToLower() == "id")
                        b.Append($"{c.Name} {ConvertPropertyType(c.PropertyType)} primary key,");
                    else
                        b.Append($"{c.Name} {ConvertPropertyType(c.PropertyType)},");
                }
                
            }

            if (b[b.Length - 1] == ',')
                b.Remove(b.Length - 1,1);

            b.Append(");");

            return ExecuteCommandLog(conn, b.ToString());
        }

        static String ConvertPropertyType(Type t)
        {
            if (t == typeof(String))
                return "text";
            else if (t.IsEnum)
                return "text";
            else
                return "numeric";
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

        public static int AddColumn(SqliteConnection conn, String tableName, String columnName, ColumnType type)
        {
            using (var cmd = conn.CreateCommand())
            {
                string columnType = "";
                if (type == ColumnType.String)
                    columnType = "text";
                else
                    columnType = "numeric";
                cmd.CommandText = $"alter table {tableName} add column {columnName} {columnType};";
                return cmd.ExecuteNonQuery();
            }
        }

        public static int AddColumnValues(SqliteConnection conn, string tableName, String columnName,
            IEnumerable<(string id, Object value)> values)
        {
            int cnt = 0;
            using (var txn = conn.BeginTransaction())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"update {tableName} set {columnName}=$value where id=$id";

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
            return foo; //ExecuteCommandLog(conn,$"create index {link1}_idx on {tableName}({link1});");
        }

        public static int CreateManyToManyLinkagesTableWithForeignKeyConstraint(SqliteConnection conn, String tableName,String link1,String link2,String parentTable,String parentColumn)
        {
            int foo = 0;
            foo = ExecuteCommandLog(conn, $@"create table {tableName} ({link1} text references {parentTable}({parentColumn}), {link2} text);create index {link1}_idx on {tableName}({link1});");
            return foo; //ExecuteCommandLog(conn,$"create index {link1}_idx on {tableName}({link1});");
        }


        public static int InsertOrUpdateBlobRows(SqliteConnection connection, String tableName, IEnumerable<Object> objs,
            Func<Object, (string, byte[])> GetIdAndBytes)
        {
            L.Trace($"Starting insert or update blob rows - {objs.Count()} objects on {tableName}");
            int cnt = 0;

            var txn = connection.BeginTransaction();

            String queryCommand = $"select rowid from {tableName} where id=($id)";
            String updateCommand = $"update {tableName} set data=$data where id=($id);";
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

        static void TrimComma(StringBuilder b)
        {
            if (b[b.Length - 1] == ',')
                b.Remove(b.Length - 1, 1);
        }

        public static int InsertRows(SqliteConnection connection, String tableName, Type t,Predicate<PropertyInfo> pis,IEnumerable<Object> objs)
        {
            L.Trace($"Starting insert or update blob rows - {objs.Count()} objects on {tableName}");
            int cnt = 0;

            StringBuilder b = new StringBuilder();

            b.Append($"insert or replace into {tableName} ");

            StringBuilder values = new StringBuilder();

            values.Append(" values (");

            StringBuilder names = new StringBuilder();

            names.Append("(");

            List<(string propertyName,MemberGetter setter,PropertyInfo pi)> propGetters = new List<(string,MemberGetter,PropertyInfo pi)>();

            foreach (var c in t.GetProperties())
            {
                if (pis(c))
                {
                    names.Append($"{c.Name},");
                    values.Append($"${c.Name},");
                    propGetters.Add((c.Name,t.DelegateForGetPropertyValue(c.Name),c));
                }

            }

            TrimComma(names);
            TrimComma(values);

            names.Append(")");
            values.Append(")");

            String insertCommand = b.ToString() + " " + names.ToString() + " " + values.ToString();

            L.Trace(insertCommand);

            var txn = connection.BeginTransaction();
            {
                foreach (var c in objs)
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        string id = "";
                        cmd.CommandText = insertCommand;
                        foreach (var g in propGetters)
                        {
                            if (g.propertyName.ToLower() == "id")
                                id = (string)g.setter(c);

                            if (g.pi.PropertyType.IsEnum)
                            {
                                cmd.Parameters.AddWithValue(g.propertyName, g.setter(c).ToString());
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(g.propertyName, g.setter(c));
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
                            L.Trace($"{cnt} objects committed on {tableName}");

                            txn.Commit();
                            txn = connection.BeginTransaction();
                        }
                    }
                }
            }
            txn.Commit();
            L.Trace($"{cnt} objects finished on {tableName}");
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

        public static IEnumerable<(string id,byte[] blob)> GetBlobs(SqliteConnection connection, String tableName, int start, int end,string sortKey="",SortTypeEnum sortType=SortTypeEnum.None)
        {
            using (var txn = connection.BeginTransaction())
            {
                String queryCommand = "";
                if (start < 0)
                    start = 0;
                if (sortKey == "" || sortType == SortTypeEnum.None)
                {
                    if (end > start)
                        queryCommand = $"select id,data from {tableName} where rowid>={start} and rowid< {end}";
                    else if (end < 0)
                        queryCommand = $"select id,data from {tableName} where rowid>={start}";
                    else
                        queryCommand = $"select id,data from {tableName} where rowid={start}";
                }
                else
                {
                    if (end > start)
                        queryCommand = $"select id,data from {tableName} where rowid>={start} and rowid< {end} sort by {sortKey} {sortType}";
                    else if (end < 0)
                        queryCommand = $"select id,data from {tableName} where rowid>={start} sort by {sortKey} {sortType}";
                    else
                        queryCommand = $"select id,data from {tableName} where rowid={start} sort by {sortKey} {sortType}";

                }

                using (var queryCmd = connection.CreateCommand())
                {
                    queryCmd.CommandText = queryCommand;

                    int cnt = 0;

                    using (var data = queryCmd.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            cnt++;
                            var id = data[0] as string;
                            var x = data[1] as byte[];
                            if (x == null)
                            {
                                L.Trace($"Invalid id - {id}");
                            }
                            yield return (id, x);
                        }
                    }

                }
                txn.Commit();
            }
        }

        public static IEnumerable<(string, byte[])> QueryId2(SqliteConnection connection, String tableName, IEnumerable<Object> objs, Func<Object, string> getMapping)
        {
            using (var txn = connection.BeginTransaction())
            {
                String queryCommand = $"select data from {tableName} where id=($id)";

                foreach (var c in objs)
                {
                    using (var queryCmd = connection.CreateCommand())
                    {
                        queryCmd.CommandText = queryCommand;

                        var q = getMapping(c);

                        queryCmd.Parameters.AddWithValue("$id", q);
                        using (var data = queryCmd.ExecuteReader())
                        {
                            if (!data.HasRows)
                            {
                                yield return (q, null);
                            }
                            else
                            {
                                yield return (q, data[0] as byte[]);
                            }
                        }

                    }
                }
                txn.Commit();
            }
        }


        public static IEnumerable<(string, bool)> QueryId(SqliteConnection connection, String tableName, IEnumerable<Object> objs, Func<Object, string> getMapping)
        {
            using (var txn = connection.BeginTransaction())
            {
                String queryCommand = $"select count(*) from {tableName} where id=($id)";

                foreach (var c in objs)
                {
                    using (var queryCmd = connection.CreateCommand())
                    {
                        queryCmd.CommandText = queryCommand;

                        var q = getMapping(c);

                        queryCmd.Parameters.AddWithValue("$id", q);
                        int cnt = Convert.ToInt32(queryCmd.ExecuteScalar());
                        yield return (q, cnt > 0);
                    }
                }
                txn.Commit();
            }
        }
        
    }
}
