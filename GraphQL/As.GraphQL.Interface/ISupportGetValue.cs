using System;
using System.Collections.Generic;
using System.Text;

namespace As.GraphQL.Interface
{
    public interface ISupportGetValue
    {
        Object GetValue(string field);
    }
}
