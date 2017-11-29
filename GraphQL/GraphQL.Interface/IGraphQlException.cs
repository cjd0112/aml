using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Interface
{
    public interface IGraphQlException
    {
        int Line { get; }
        int Column { get; }

        
    }
}
