using System;
using System.Collections.Generic;
using System.Reflection;
using As.GraphQL;
using As.GraphQL.Interface;
using Microsoft.Data.Sqlite;

namespace As.GraphDB
{
   
   

    public abstract class RepositoryGraphDbBase 
    {
        protected SqliteConnection conn;

        private Type topLevelType;
        protected abstract Object ResolveTopLevelType(Type t);

        protected RepositoryGraphDbBase(SqliteConnection conn,Type topLevelType)
        {
            this.conn = conn;
            this.topLevelType = topLevelType;
        }

        public Object Run(IGraphQuery query,GraphQlCustomiseSchema schema=null)
        {
            return new GraphQlDocument(query.Query)
                .CustomiseSchema(schema ?? new GraphQlCustomiseSchema())
                .Validate(topLevelType)
                .Run(ResolveTopLevelType(topLevelType),query.OperationName,query.Variables)
                .GetOutput();
        }

    
   
    }
}
