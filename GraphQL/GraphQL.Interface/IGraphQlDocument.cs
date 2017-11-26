using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Interface
{
    public interface IGraphQlDocument
    {
        IGraphQlDocument CustomiseSchema(IGraphQlCustomiseSchema custom);

        IGraphQlDocument Validate(Type topLevelType);

        IGraphQlDocument Run(Object topLevelObject);

        String GetOutput();
    }
}
