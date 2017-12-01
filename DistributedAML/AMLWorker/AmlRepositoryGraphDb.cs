using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Google.Protobuf;
using GraphQL;
using GraphQL.Interface;
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
    }

    public class AmlRepositoryGraphDb : IGraphQlDatabase
    {
        private SqliteConnection conn;

        public AmlRepositoryGraphDb(SqliteConnection conn)
        {
            this.conn = conn;

        }

        public Object Run(String query)
        {
            return new GraphQlDocument(query)
                .Validate(typeof(AmlRepositoryQuery))
                .Run(new AmlRepositoryQuery(), this)
                .GetOutput();
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
                foreach (var c in SqlHelper.GetBlobs(conn, "Accounts", 10, 200))
                {
                    yield return Account.Parser.ParseFrom(c);
                }
            }
        }
    }
}
