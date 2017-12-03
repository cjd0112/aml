using System;
using System.Collections.Generic;

namespace As.GraphQL.Interface
{
    public interface IGraphQlDatabase
    {
        bool SupportField(Object parentObject, String fieldName);
        IEnumerable<Object> ResolveFieldValue(Object parentObject, String fieldName, Dictionary<string, Object> argumentValues);
    }
}
