using System;
using System.Data;
using As.GraphQL.Interface;

namespace As.GraphDB.Sql
{
    public class DataRecordHelper<T> : ISupportGetValue
    {
        public SqlitePropertiesAndCommands<T>  propertiesAndCommands { get; set; }   
        public IDataRecord Record { get; set; }
        public int OrdinalInQuery { get; set; }

        public DataRecordHelper(SqlitePropertiesAndCommands<T> propertiesAndCommands, IDataRecord record)
        {
            this.propertiesAndCommands = propertiesAndCommands;
            Record = record;
        }


        public T GetObject()
        {
            return propertiesAndCommands.CreateInstance(this);
        }

        public Object GetValue(String field)
        {
            return Record[field];
        }

        public DataRecordHelper<T> IncrementCount()
        {
            OrdinalInQuery++;
            return this;
        }

    }
}
