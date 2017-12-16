using System;

namespace As.GraphDB
{
    public interface IGraphQuery
    {
        String OperationName { get; }
        String Query { get; }
        String Variables { get; }
    }
}