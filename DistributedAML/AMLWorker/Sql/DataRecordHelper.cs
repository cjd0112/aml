using System;
using System.Data;
using As.GraphQL.Interface;

namespace AMLWorker.Sql
{
    public class DataRecordHelper : ISupportGetValue
    {
        public Type UnderlyingType { get; set; }   
        public IDataRecord Record { get; set; }
        public int OrdinalInQuery { get; set; }

        public DataRecordHelper(int ordinalInQuery,Type t, IDataRecord record)
        {
            UnderlyingType = t;
            Record = record;
            OrdinalInQuery = ordinalInQuery;
        }


        public Object GetValue(String field)
        {
            return Record[field];
        }

    }
}
