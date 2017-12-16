using System;

namespace As.GraphDB.Sql
{
    public enum SortTypeEnum
    {
        None,
        Asc,
        Desc,
    }

    public class Sort
    {
        public String SortField { get; set; }
        public SortTypeEnum SortType { get; set; }

    }
    
}
