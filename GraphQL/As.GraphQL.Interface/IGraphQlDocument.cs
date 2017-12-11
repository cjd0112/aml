using System;

namespace As.GraphQL.Interface
{
    public interface IGraphQlDocument
    {
        IGraphQlDocument CustomiseSchema(GraphQlCustomiseSchema custom);

        IGraphQlDocument Validate(Type queryType);

        IGraphQlDocument Run(Object topLevelObject,String operationName,String variables);

        Object GetOutput();
    }
}
