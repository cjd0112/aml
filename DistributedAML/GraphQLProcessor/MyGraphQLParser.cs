using System;
using Antlr4.Runtime;
using GraphQLInterface.GraphQLType;

namespace GraphQL
{
    public class MyGraphQLParser
    {
        public static void ProcessGraphQLDocument(String query,DatabaseCollection coll,IGraphQLOutput output)
        {
            var lexer = new GraphQLLexer(new AntlrInputStream(query));

            var cts = new CommonTokenStream(lexer);

            GraphQLParser parser = new GraphQLParser(cts);

            var doc = new GraphQLDocument(parser.document());

            var schemaContainer = SchemaLoader.GetSchemaContainer();
            doc.Validate(schemaContainer);

            doc.Process(schemaContainer, coll,output);
        }
    }
}
