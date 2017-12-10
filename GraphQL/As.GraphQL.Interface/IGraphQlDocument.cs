using System;

namespace As.GraphQL.Interface
{
    public interface IGraphQlDocument
    {
        IGraphQlDocument CustomiseSchema(GraphQlCustomiseSchema custom);

        IGraphQlDocument Validate(Type queryType,Type mutationType);

        IGraphQlDocument Run(Object topLevelObject,IGraphQlDatabase db=null);

        Object GetOutput();
    }
}
