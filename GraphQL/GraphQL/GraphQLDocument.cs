using System;
using Antlr4.Runtime;
using GraphQL.Interface;
using GraphQL.Interface.GraphQLSerializer;
using GraphQLInterface.GraphQLType;

namespace GraphQL
{
    public class GraphQlDocument : IGraphQlDocument
    {
        private GraphQLParser.DocumentContext documentContext;
        private IGraphQlSchema schema;
        private IGraphQlOutput output;
        public GraphQlDocument(String query)
        {
            var lexer = new GraphQLLexer(new AntlrInputStream(query));

            var cts = new CommonTokenStream(lexer);

            GraphQLParser parser = new GraphQLParser(cts);

            documentContext = parser.document();

        }

        public GraphQLParser.DocumentContext GetDocumentContext()
        {
            return documentContext;
        }

        public IGraphQlDocument Validate(IGraphQlSchema schema)
        {
            var z = new GraphQlMainValidation(this,schema);
            this.schema = schema;
            return this;
        }

        public IGraphQlDocument Run(Object topLevelObject)
        {
            if (schema == null)
                throw new Exception("Need to call 'validate' first");

            output = new GraphlLOutput();
            var p = new GraphQlMainExecution(this,schema,output,topLevelObject);
            return this;
        }

        public String GetOutput()
        {
            return output.ToString();
        }
    }
}
