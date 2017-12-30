using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using As.Logger;
using As.Shared;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Serialization;

namespace As.GraphDB.Sql
{
    public struct SqlPredicate
    {
        public SqlPredicate(String pn,Object pv)
        {
            propertyName = pn;
            propertyValue = pv;

        }
        public String propertyName;
        public Object propertyValue;
    }

    public class SqlInnerJoin<T, Y>
    {
        private SqlitePropertiesAndCommands t1;
        private SqlitePropertiesAndCommands t2;

        public SqlInnerJoin(Func<Type,SqlitePropertiesAndCommands> resolver)
        {
            this.t1 = resolver(typeof(T));
            this.t2 = resolver(typeof(Y));
        }


        String JoinClause(string t1JoinProperty, string t2JoinProperty)
        {
            return
                $"select A.*, B.* from {t1.tableName} as A inner join {t2.tableName} as B on A.{t1JoinProperty} = B.{t2JoinProperty} ";
        }

        IEnumerable<String> WhereClause(TypeContainer tc, IEnumerable<SqlPredicate> t1Predicates)
        {
            if (t1Predicates.Any())
            {
                StringBuilder foo = new StringBuilder();
                foreach (var c in t1Predicates)
                {

                    PropertyContainer pc = tc.GetProperty(c.propertyName);
                    if (pc == null)
                        throw new Exception($"Could not find property - {c.propertyName} on type - {t1.tableName} for join operation");

                    if (pc.PropertyType != typeof(String))
                        throw new Exception(
                            $"Currently join only works on strings ... - {c.propertyName} - {t1.tableName}");
                    // assume string for now ... 
                    if (tc == t1.typeContainer)
                        foo.Append($" A.{c.propertyName} = '{c.propertyValue.ToString()} ");
                    else
                        foo.Append($" B.{c.propertyName} = '{c.propertyValue.ToString()} ");
                    yield return foo.ToString();
                }
            }
        }


        public IEnumerable<(T t1, Y t2)> JoinT1Predicate(SqliteConnection conn, string joinProperty, params SqlPredicate[] t1Predicate)
        {
            return Join(conn, joinProperty, t1Predicate, new SqlPredicate[] { });
        }

        public IEnumerable<(T t1, Y t2)> JoinT2Predicate(SqliteConnection conn, string joinProperty, params SqlPredicate[] t2Predicate)
        {
            return Join(conn, joinProperty, new SqlPredicate[] { }, t2Predicate);
        }



        public IEnumerable<(T t1, Y t2)> Join(SqliteConnection conn,string joinProperty, IEnumerable<SqlPredicate> t1Predicates,
            IEnumerable<SqlPredicate> t2Predicates)
        {
            StringBuilder query = new StringBuilder();
            using (var cmd = conn.CreateCommand())
            {
                query.Append(JoinClause(joinProperty, joinProperty));

                var t1WhereClauses = WhereClause(t1.typeContainer, t1Predicates).ToArray();

                var t2WhereClauses = WhereClause(t2.typeContainer, t2Predicates).ToArray();

                if (t1WhereClauses.Any() || t2WhereClauses.Any())
                {
                    query.Append(" where ");
                    foreach (var c in t1WhereClauses)
                    {
                        query.Append(c);
                        query.Append(" and ");
                    }

                    foreach (var c in t2WhereClauses)
                    {
                        query.Append(c);
                        query.Append(" and ");
                    }

                    query.Remove(query.Length - " and ".Length," and ".Length);

              
                }
                query.Append(";");

                cmd.CommandText = query.ToString();
                L.Trace($"Running join command - {cmd.CommandText}");

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var t1 = new DataRecordHelper<T>(this.t1, rdr).GetObject();
                        var y1 = new DataRecordHelper<Y>(this.t2, rdr).GetObject();
                        yield return (t1, y1);
                    }
                }
            }
        }
    }
}
