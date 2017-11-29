using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using GraphQL.GraphQLType;
using GraphQL.Interface;

namespace GraphQL
{
    public class GraphQlMainBase
    {
        protected GraphQLParser.DocumentContext documentContext;
        protected __SchemaContainer schema;

        protected GraphQlMainBase(IGraphQlDocument document, __SchemaContainer schema)
        {
            this.schema = schema;
            this.documentContext = ((GraphQlDocument) document).GetDocumentContext();
        }


        public void Error(string error, ParserRuleContext context)
        {
            if (context == null)
                throw new GraphQlException($"Error processing query - {error}",0,0);
            throw new GraphQlException($"Error processing query - {error}, Line - {context.Start.Line}, Column - {context.Start.Column}",context.Start.Line,context.Start.Column);
        }

        protected GraphQLParser.OperationDefinitionContext GetTopLevelOperation(String operationName = "")
        {
            var anonymousOperations = GetOperations(x => x.NAME() == null).FirstOrDefault();
            if (anonymousOperations != null)
                return anonymousOperations;

            if (operationName != "")
            {
                var namedOperation = GetOperations(x => x.NAME().GetText() == operationName).FirstOrDefault();
                if (namedOperation == null)
                    Error($"Could not find named operation - {operationName} in document",documentContext);

                return namedOperation;
            }

            var oneOperation = GetOperations(x => x.NAME() != null).FirstOrDefault();
            if (oneOperation == null)
                Error($"More than one named operation but no indication of which operation to select",documentContext);

            return oneOperation;

        }

        protected IEnumerable<GraphQLParser.OperationDefinitionContext> GetOperations(Predicate<GraphQLParser.OperationDefinitionContext> filter)
        {
            return documentContext.definition()
                .Where(x => x.operationDefinition() != null)
                .Select(x => x.operationDefinition()).Where(x => filter(x)).ToArray();

        }

        protected IEnumerable<GraphQLParser.FragmentDefinitionContext> GetFragments(
            Predicate<GraphQLParser.FragmentDefinitionContext> filter)
        {
            return documentContext.definition().
                Where(x => x.fragmentDefinition() != null)
                .Select(x => x.fragmentDefinition()).Where(x => filter(x)).ToArray();
        }
    }
}
