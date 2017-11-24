using GraphQLInterface.GraphQLType;

namespace GraphQL
{
    public class GraphQLDocument
    {
        private GraphQLParser.DocumentContext documentContext;
        public GraphQLDocument(GraphQLParser.DocumentContext documentContext)
        {
            this.documentContext = documentContext;

        }

        public void Validate(__SchemaContainer schema)
        {
            var vm = new ValidationManager(documentContext, schema);
        }
        public void Process(__SchemaContainer schema, DatabaseCollection db, IGraphQLOutput output)
        {
            ExecutionManager2 em = new ExecutionManager2(documentContext,schema,new Query(),new GraphQLToDatabaseAdapter(db,output));
        }
    }
}
