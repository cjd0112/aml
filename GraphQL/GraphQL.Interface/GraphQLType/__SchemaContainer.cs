using GraphQL.Interface;

namespace GraphQLInterface.GraphQLType
{
    public class __SchemaContainer : IGraphQlSchema
    {
        public __SchemaContainer(__Schema s)
        {
            __schema = s;
        }
        public __Schema __schema { get; set; }

        public __Type GetType(string name)
        {
            return __schema.GetType(name);
        }
    }
}
