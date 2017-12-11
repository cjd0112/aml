using System;
using System.Collections.Generic;
using AMLWorker.Sql;
using Microsoft.Data.Sqlite;

namespace AMLWorker.A4A
{

    public class A4ARepositoryGraphDb : RepositoryGraphDbBase
    {
        private SqlitePropertiesAndCommands<A4AParty> partySql = new SqlitePropertiesAndCommands<A4AParty>();
        private SqlitePropertiesAndCommands<A4ACategory> categorySql = new SqlitePropertiesAndCommands<A4ACategory>();
        private SqlitePropertiesAndCommands<A4AMessage> messageSql = new SqlitePropertiesAndCommands<A4AMessage>();


        public A4ARepositoryGraphDb(SqliteConnection conn,
            SqlitePropertiesAndCommands<A4AParty> partySql,
            SqlitePropertiesAndCommands<A4ACategory> categorySql,
            SqlitePropertiesAndCommands<A4AMessage> messageSql) : base(conn,typeof(A4AQuery))
        {
            this.partySql = partySql;
            this.categorySql = categorySql;
            this.messageSql = messageSql;
        }

    
        public IEnumerable<A4AMessage> SearchMessages(string search, Range range, Sort sort)
        {
            foreach (var c in SqlTableHelper.SelectData(conn, messageSql, range,sort))
            {
                yield return c.GetObject();
            }

        }

        protected override object ResolveTopLevelType(Type t)
        {
            return new A4AQuery(new A4ARepositoryQuery(this), new A4AMutations());
        }
    }
}
