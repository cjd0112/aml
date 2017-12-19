using System;
using System.Collections.Generic;
using System.Linq;
using As.GraphDB;
using As.GraphDB.Sql;
using Microsoft.Data.Sqlite;

namespace As.A4ACore
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
            foreach (var c in new SqlTableWithId().SelectData(conn, messageSql, "", range,sort))
            {
                yield return c.GetObject();
            }

        }

        public A4AMessage AddMessage(A4AMessageSetter setter)
        {
            var sqlTableWithId = new SqlTableWithId();
            int foo = sqlTableWithId.InsertOrReplace(conn, messageSql, new []{new A4AMessage{Content = setter.Message,Id=setter.Id}});

            var z = sqlTableWithId.SelectData(conn, messageSql, $" id like '{setter.Id}'", new Range(), new Sort()).Select(x=>x.GetObject()).FirstOrDefault();

            if (z == null)
                throw new Exception($"Could not find recently added object with id - {setter.Id}");

            return z;
        }

        protected override object ResolveTopLevelType(Type t)
        {
            return new A4AQuery(new A4ARepositoryQuery(this), new A4AMutations(this));
        }
    }
}
