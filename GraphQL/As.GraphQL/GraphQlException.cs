using System;
using As.GraphQL.Interface;

namespace As.GraphQL
{
    public class GraphQlException : Exception,IGraphQlException
    {
        public int Line { get; private set; }
        public int Column { get; private set; }
        public GraphQlException(Exception e, int line, int column) : base(e.Message,e)
        {
            this.Line = line;
            this.Column = column;
        }

        public GraphQlException(String e, int line, int column) : base(e)
        {
            this.Line = line;
            this.Column = column;
        }

    }
}
