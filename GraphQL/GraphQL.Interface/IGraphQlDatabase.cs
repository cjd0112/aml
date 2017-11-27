using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Interface
{
    public interface IGraphQlDatabase
    {
        bool IsTopLevelQueryFieldSupported(string fieldName, Object topLevelObject);
    }
}
