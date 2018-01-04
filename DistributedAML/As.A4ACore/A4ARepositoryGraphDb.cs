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
        private SqlTableWithPrimaryKey messageSql;
        public A4ARepositoryGraphDb(SqliteConnection conn,
            SqlTableWithPrimaryKey messageSql) : base(conn,typeof(A4AQuery))
        {
            this.messageSql = messageSql;
        }

    
        public IEnumerable<A4AMessage> SearchMessages(string search, Range range, Sort sort)
        {
            foreach (var c in messageSql.SelectData<A4AMessage>(conn, "", range,sort))
            {
                yield return c.GetObject();
            }

        }

        public A4AMessage AddMessage(A4AMessageSetter setter)
        {
            int foo = messageSql.InsertOrReplace(conn, new []{new A4AMessage{TextContent = setter.Message,MessageId=setter.Id}});

            var z = messageSql.SelectData<A4AMessage>(conn, $" MessageId like '{setter.Id}'", new Range(), new Sort()).Select(x=>x.GetObject()).FirstOrDefault();

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
