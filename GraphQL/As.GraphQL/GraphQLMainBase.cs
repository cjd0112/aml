using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using As.GraphQL.GraphQLType;
using As.GraphQL.Interface;

namespace As.GraphQL
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


        protected Object ExtractAppropriateObjectForOperation(Object topLevelObject,GraphQLParser.OperationDefinitionContext context)
        {
            if (context.operationType().GetText().ToLower() == "mutation")
            {
                var property = topLevelObject.GetType().GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttribute<GraphQlMutationsAttribute>() != null);
                if (property == null)
                    Error($"Could not find an object with 'GraphQlMutationsAttribute' on property to support mutation - {context.NAME().GetText()}",context);

                return property.GetValue(topLevelObject);
            }
            else if (context.operationType().GetText().ToLower() == "query")
            {
                var property = topLevelObject.GetType().GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttribute<GraphQlTopLevelQueryAttribute>() != null);
                if (property == null)
                    Error($"Could not find an object with 'GraphQlQueryAttribute' on property to support query - {context.NAME().GetText()}", context);

                return property.GetValue(topLevelObject);
            }
            else
            {
                Error($"Unexpected operation type - expected 'query' or 'mutation' received - {context.operationType().GetText()}",context);
                return null;
            }

        }

        protected __Type ExtractAppropriate__TypeForOperation(__Schema schema, GraphQLParser.OperationDefinitionContext context)
        {
            if (context.operationType().GetText().ToLower() == "mutation")
            {
                return schema.mutationType;
            }
            else if (context.operationType().GetText().ToLower() == "query")
            {
                return schema.queryType;
            }
            else
            {
                Error($"Unexpected operation type - expected 'query' or 'mutation' received - {context.operationType().GetText()}", context);
                return null;
            }

        }
    }
}
