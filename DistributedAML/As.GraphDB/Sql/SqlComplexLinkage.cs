using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace As.GraphDB.Sql
{
    public class SqlComplexLinkage<T, Y> where T:struct,IConvertible where Y:struct,IConvertible
    {
        public SqlComplexLinkage(T fromType, String fromId, T toType, String toId, Y relationType)
        {

            FromType = fromType;
            FromId = fromId;
            ToId = toId;
            ToType = toType;
            RelationType = relationType;
        }

        public SqlComplexLinkage(IDataRecord rec)
        {
            FromType = (T)Enum.Parse(typeof(T), rec["FromType"].ToString());
            FromId = rec["FromId"].ToString();
            ToType = (T)Enum.Parse(typeof(T), rec["ToType"].ToString());
            ToId = rec["ToId"].ToString();
            RelationType = (Y) Enum.Parse(typeof(Y), rec["RelationType"].ToString());
        }

        public T FromType { get; set; }
        public String FromId { get; set; }
        public T ToType { get; set; }
        public String ToId { get; set; }
        public Y RelationType { get; set; }


    }

}
