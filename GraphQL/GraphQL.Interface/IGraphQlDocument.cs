using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Interface
{
    public interface IGraphQlDocument
    {

        IGraphQlDocument Validate(IGraphQlSchema schema);

        IGraphQlDocument Run(Object topLevelObject);

        String GetOutput();
    }
}
