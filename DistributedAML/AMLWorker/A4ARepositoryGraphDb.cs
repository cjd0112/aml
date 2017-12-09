using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AMLWorker.Sql;
using As.GraphQL;
using As.GraphQL.Interface;
using Fasterflect;
using Google.Protobuf;
using GraphQL;
using Microsoft.Data.Sqlite;

namespace AMLWorker
{
    public class A4ARepositoryQuery
    {
        public IEnumerable<A4AMessage> Messages { get; set; }
        public IEnumerable<A4ACategory> Categories { get; set; }
        public IEnumerable<A4AParty> Parties { get; set; }

    }

    public class A4ARepositoryGraphDb : RepositoryGraphDbBase
    {
        private SqlitePropertiesAndCommands<A4AParty> partySql = new SqlitePropertiesAndCommands<A4AParty>();
        private SqlitePropertiesAndCommands<A4ACategory> categorySql = new SqlitePropertiesAndCommands<A4ACategory>();
        private SqlitePropertiesAndCommands<A4AMessage> messageSql = new SqlitePropertiesAndCommands<A4AMessage>();


        public A4ARepositoryGraphDb(SqliteConnection conn,
            SqlitePropertiesAndCommands<A4AParty> partySql,
            SqlitePropertiesAndCommands<A4ACategory> categorySql,
            SqlitePropertiesAndCommands<A4AMessage> messageSql) : base(conn,typeof(A4ARepositoryQuery))
        {
            this.partySql = partySql;
            this.categorySql = categorySql;
            this.messageSql = messageSql;
        }

   

        public override bool SupportField(object parentObject, string fieldName)
        {
            if (parentObject is A4ARepositoryQuery && fieldName == "Messages" || fieldName == "Categories" ||
                fieldName == "Parties")
                return true;
            return false;
        }

        public override IEnumerable<object> ResolveFieldValue(object parentObject, string fieldName,
            Dictionary<string, object> argumentValues)
        {
            if (parentObject is AmlRepositoryQuery && fieldName == "Parties")
            {
                foreach (var c in SqlTableHelper.SelectData(conn, partySql, GetRange(argumentValues),
                    GetSort(argumentValues)))
                {
                    yield return c;
                }
            }
        }
    }
}
