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

        public override bool IncludeMethod(MethodInfo mi)
        {
            // don't include any IMessage functions 
            if (typeof(IMessage).IsAssignableFrom(mi.DeclaringType))
                return false;
            return true;
        }

        public override IEnumerable<(string fieldName, string description, Type fieldType)> AddAdditionalFields(Type type)
        {
            return base.AddAdditionalFields(type);
        }
    }

    public class RepositoryGraphDbBase : IGraphQlDatabase
    {
        protected SqliteConnection conn;

        private Type topLevelType;
        private Func<Type, Object> resolver;
        protected RepositoryGraphDbBase(SqliteConnection conn,Type topLevelType,Func<Type,Object> resolver)
        {
            this.conn = conn;
            this.topLevelType = topLevelType;
            this.resolver = resolver;
        }

        public Object Run(String query)
        {
            return new GraphQlDocument(query)
                .CustomiseSchema(new CustomizeSchema())
                .Validate(topLevelType)
                .Run(resolver(topLevelType), this)
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
