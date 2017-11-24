using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLInterface
{
    public enum RangeType
    {
        None,
        Random
    }
    [GraphQL(GraphQLAttributeTypeEnum.TreatAsInputObject)]
    public class Range
    {
        public int Start { get; set; }
        public int PageSize { get; set; }
        public RangeType RangeType { get; set; }
    }
}
