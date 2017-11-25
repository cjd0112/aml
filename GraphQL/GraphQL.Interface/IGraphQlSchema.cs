using System;
using System.Collections.Generic;
using System.Text;
using GraphQLInterface.GraphQLType;

namespace GraphQL.Interface
{
    public interface IGraphQlSchema
    {
        __Schema __schema { get; set; }
        __Type GetType(string name);
    }
}
