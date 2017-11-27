using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLInterface
{
    public enum SortTypeEnum
    {
        None,
        Ascending,
        Descending
    }

    [GraphQL(GraphQLAttributeTypeEnum.TreatAsInputObject)]
    public class Sort
    {
        public String SortField { get; set; }
        public SortTypeEnum SortType { get; set; }

    }
}
