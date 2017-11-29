using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using GraphQL.Interface;

namespace GraphQL
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
    }
}
