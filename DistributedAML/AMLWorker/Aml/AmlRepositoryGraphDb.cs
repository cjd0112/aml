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
    public class AmlQuery
    {
        [GraphQlTopLevelQuery]
        public AmlRepositoryQuery Query { get; set; }
    }

    public class AmlRepositoryQuery
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Party> Parties { get; set; }
        

    }

    public class AmlRepositoryGraphDb : RepositoryGraphDbBase
    {
        private SqlitePropertiesAndCommands<Party> partySql = new SqlitePropertiesAndCommands<Party>();
        private SqlitePropertiesAndCommands<Account> accountSql = new SqlitePropertiesAndCommands<Account>();
        private SqlitePropertiesAndCommands<Transaction> transactionSql = new SqlitePropertiesAndCommands<Transaction>();


        public AmlRepositoryGraphDb(SqliteConnection conn,
            SqlitePropertiesAndCommands<Party> partySql,
            SqlitePropertiesAndCommands<Account> accountSql,
            SqlitePropertiesAndCommands<Transaction> transactionSql) : base(conn,typeof(AmlRepositoryQuery))
        {
            this.partySql = partySql;
            this.accountSql = accountSql;
            this.transactionSql = transactionSql;
        }


        protected override object ResolveTopLevelType(Type t)
        {
            return new AmlRepositoryQuery();
        }
    }
}
