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

  

    public class CustomizeSchema : GraphQlCustomiseSchema
    {
        public override bool IncludeProperty(PropertyInfo pi)
        {
            if (typeof(IMessage).IsAssignableFrom(pi.DeclaringType))
            {
                if (pi.PropertyType.Name.StartsWith("MessageParser") ||
                    pi.PropertyType.Name == "MessageDescriptor")
                    return false;
            }

            return true;
        }

        public override IEnumerable<(string fieldName, string description, Type fieldType)> AddAdditionalFields(Type type)
        {
            return base.AddAdditionalFields(type);
        }

        public override IEnumerable<(string inputName, Type inputType)> GetInputValues(string fieldName, PropertyInfo pi)
        {
            foreach (var c in base.GetInputValues(fieldName, pi))
                yield return c;

            if (pi.DeclaringType == typeof(AmlRepositoryQuery) &&   pi.PropertyType == typeof(IEnumerable<Account>))
            {
                yield return ("Range", typeof(Range));
                yield return ("Sort", typeof(Sort));

            }
        }
    }

    public class AmlRepositoryGraphDb : IGraphQlDatabase
    {
        private SqliteConnection conn;
        private SqlitePropertiesAndCommands<Party> partySql = new SqlitePropertiesAndCommands<Party>();
        private SqlitePropertiesAndCommands<Account> accountSql = new SqlitePropertiesAndCommands<Account>();
        private SqlitePropertiesAndCommands<Transaction> transactionSql = new SqlitePropertiesAndCommands<Transaction>();


        public AmlRepositoryGraphDb(SqliteConnection conn,
            SqlitePropertiesAndCommands<Party> partySql,
            SqlitePropertiesAndCommands<Account> accountSql,
            SqlitePropertiesAndCommands<Transaction> transactionSql)
        {
            this.conn = conn;
            this.partySql = partySql;
            this.accountSql = accountSql;
            this.transactionSql = transactionSql;
        }

        public Object Run(String query)
        {
            return new GraphQlDocument(query)
                .CustomiseSchema(new CustomizeSchema())
                .Validate(typeof(AmlRepositoryQuery))
                .Run(new AmlRepositoryQuery(), this)
                .GetOutput();
        }

        Range GetRange(Dictionary<string, object> arguments)
        {
            if (arguments.TryGetValue("Range", out Object o))
            {
                return (Range) o;
            }
            return new Range();

        }

        Sort GetSort(Dictionary<string, object> arguments)
        {
            if (arguments.TryGetValue("Sort", out Object o))
            {
                return (Sort) o;
            }
            return new Sort();
        }

        public bool SupportField(object parentObject, string fieldName)
        {
            if (parentObject is AmlRepositoryQuery && fieldName == "Transactions" || fieldName == "Accounts" ||
                fieldName == "Parties")
                return true;
            return false;
        }

        public IEnumerable<object> ResolveFieldValue(object parentObject, string fieldName,
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
