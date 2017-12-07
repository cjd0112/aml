using System;
using System.Data;
using As.GraphQL.Interface;

namespace AMLWorker.Sql
{
    public class DataRecordHelper : ISupportGetValue
    {
        Type UnderlyingType { get; set; }   
        IDataRecord Record { get; set; }

        public DataRecordHelper(Type t, IDataRecord record)
        {
            UnderlyingType = t;
            Record = record;
        }


        public Object GetValue(String field)
        {
            return Record[field];
        }
    }
}
