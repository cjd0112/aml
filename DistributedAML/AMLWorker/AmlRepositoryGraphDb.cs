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


        public override bool SupportField(object parentObject, string fieldName)
        {
            if (parentObject is AmlRepositoryQuery && fieldName == "Transactions" || fieldName == "Accounts" ||
                fieldName == "Parties")
                return true;
            return false;
        }

        public override IEnumerable<object> ResolveFieldValue(object parentObject, string fieldName,
            Dictionary<string, object> argumentValues)
        {
            if (parentObject is AmlRepositoryQuery && fieldName == "Accounts")
            {
                foreach (var c in SqlTableHelper.SelectData(conn, accountSql, GetRange(argumentValues),
                    GetSort(argumentValues)))
                {
                    yield return c;
                }
            }
        }
    }
}
