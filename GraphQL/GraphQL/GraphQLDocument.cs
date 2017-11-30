using System;
using Antlr4.Runtime;
using GraphQL.GraphQLSerializer;
using GraphQL.GraphQLType;
using GraphQL.Interface;
using Newtonsoft.Json.Linq;

namespace GraphQL
{
    public class GraphQlDocument : IGraphQlDocument
    {
        private GraphQLParser.DocumentContext documentContext;
        private IGraphQlOutput output;
        private GraphQlCustomiseSchema custom;
        private __SchemaContainer schema;
        private bool validation_errors = false;
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

        public IGraphQlDocument CustomiseSchema(GraphQlCustomiseSchema custom)
        {
            this.custom = custom;
            return this;
        }


        public IGraphQlDocument Validate(Type topLevelType)
        {
            if (custom == null)
                custom = new GraphQlCustomiseSchema();

            try
            {
                schema = GraphQlSchemaLoader.GetSchema(topLevelType);

                if (schema == null)
                    schema = GraphQlSchemaLoader.InitializeSchema(topLevelType,custom);

                var z = new GraphQlMainValidation(this,schema);

            }
            catch (GraphQlException e)
            {
                validation_errors = true;
                output = new GraphQlJObject();
                output.AddException(e);
            }
            catch (Exception e)
            {
                validation_errors = true;
                output = new GraphQlJObject();
                output.AddException(new GraphQlException(e,0,0));
            }
            return this;
        }

        public IGraphQlDocument Run(Object topLevelObject,IGraphQlDatabase db=null)
        {
            if (validation_errors)
                return this;
            
            if (schema == null)
                throw new Exception("Need to call 'validate' first");

            try
            {
                output = new GraphQlJObject();
                var p = new GraphQlMainExecution(this, schema, output, topLevelObject,db);
            }
            catch (GraphQlException e)
            {
                output = new GraphQlJObject();
                output.AddException(e);
            }
            catch (Exception e)
            {
                output = new GraphQlJObject();
                output.AddException(new GraphQlException(e,0,0));
            }
            return this;
        }

        public Object GetOutput()
        {
            return output.GetRoot();
        }
    }
}
