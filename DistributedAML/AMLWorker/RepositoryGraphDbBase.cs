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

    public class RepositoryGraphDbBase : IGraphQlDatabase
    {
        protected SqliteConnection conn;

        private Type queryType;
        private Type mutationType;
        protected RepositoryGraphDbBase(SqliteConnection conn,Type queryType,Type mutationType=null)
        {
            this.conn = conn;
            this.queryType = queryType;
            this.mutationType = mutationType;
        }

        public Object Run(String query)
        {
            return new GraphQlDocument(query)
                .CustomiseSchema(new CustomizeSchema())
                .Validate(queryType,mutationType)
                .Run(Activator.CreateInstance(queryType), this)
                .GetOutput();
        }

        protected Range GetRange(Dictionary<string, object> arguments)
        {
            if (arguments.TryGetValue("Range", out Object o))
            {
                return (Range) o;
            }
            return new Range();

        }

        protected Sort GetSort(Dictionary<string, object> arguments)
        {
            if (arguments.TryGetValue("Sort", out Object o))
            {
                return (Sort) o;
            }
            return new Sort();
        }

        public virtual bool SupportField(object parentObject, string fieldName)
        {
            return false;
        }

        public virtual IEnumerable<object> ResolveFieldValue(object parentObject, string fieldName,Dictionary<string, object> argumentValues)
        {
            yield break;
        }
    }
}
