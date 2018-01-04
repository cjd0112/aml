using System;
using System.Collections.Generic;
using System.Text;
using As.Shared;
using Google.Protobuf.WellKnownTypes;
using Enum = System.Enum;

namespace As.GraphDB.Sql
{
    public class SqlPropertyConverter
    {
        private PropertyContainer prop = null;
        public SqlPropertyConverter(PropertyContainer foo)
        {
            this.prop = foo;

        }

        public PropertyContainer Underlying => prop;

        public String Name => prop.Name;
        public System.Type PropertyType => prop.PropertyType;

        public Object GetValue(object owningObject)
        {
            return ConvertToDbType(prop.GetValue(owningObject));
        }

        public void SetValue(object owningObject,object value)
        {
            prop.SetValue(owningObject, ConvertFromDbType(value));
        }

        private Object ConvertToDbType(Object i)
        {
            if (i == null)
                return DBNull.Value;

            Object ret = null;
            switch (i)
            {
                case Timestamp time:
                    ret = time.Seconds;
                    break;
                case Boolean b:
                    ret = b ? "true" : "false";
                    break;
                case Enum e:
                    ret = i.ToString();
                    break;
                default:
                    ret = i;
                    break;
            }

            return ret;
        }

        // probably a bit more to do here
        private Object ConvertFromDbType(Object i)
        {
            if (i == null)
            {
                if (prop.PropertyType == typeof(String))
                    return "";
                else
                    return 0;
            }

            Object ret = null;
            switch (i)
            {
                case String s:
                    if (prop.PropertyType == typeof(String))
                        ret = s;
                    else if (prop.PropertyType == typeof(Boolean))
                    {
                        if (s.ToLower() == "true")
                            ret = true;
                        else if (s.ToLower() == "false")
                            ret = false;
                        else
                        {
                            var i2 = System.Convert.ToInt32(s);
                            if (i2 != 0)
                                ret = true;
                            else
                                ret = false;
                        }
                    }
                    else if (prop.PropertyType.IsEnum)
                        ret = Enum.Parse(prop.PropertyType, s);
                    else if (prop.PropertyType == typeof(Int32))
                        ret = System.Convert.ToInt32(s);
                    else if (prop.PropertyType == typeof(Int64))
                        ret = System.Convert.ToInt64(s);
                    else if (prop.PropertyType == typeof(Timestamp))
                    {
                        if (String.IsNullOrEmpty(s))
                            ret = new Timestamp {Seconds = 0};
                        else
                            ret = new Timestamp {Seconds = Int64.Parse(s)};
                    }
                    else
                        ret = i;
                    break;
                case Int32 q:
                    if (prop.PropertyType == typeof(Int32))
                        ret = q;
                    else if (prop.PropertyType == typeof(String))
                        ret = q.ToString();
                    else if (prop.PropertyType == typeof(Int64))
                        ret = Convert.ToInt64(q);
                    else if (prop.PropertyType.IsEnum)
                        ret = q;
                    else if (prop.PropertyType == typeof(Timestamp))
                        ret = Timestamp.FromDateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(q));
                     else if (prop.PropertyType == typeof(bool))
                         ret = q > 0?true:false;
                    break;
                case Int64 q:
                    if (prop.PropertyType == typeof(Int32))
                        ret = Convert.ToInt32(q);
                    else if (prop.PropertyType == typeof(String))
                        ret = q.ToString();
                    else if (prop.PropertyType == typeof(Int64))
                        ret = q;
                    else if (prop.PropertyType.IsEnum)
                        ret = Convert.ToInt32(q);
                    else if (prop.PropertyType == typeof(Timestamp))
                        ret = Timestamp.FromDateTimeOffset(DateTimeOffset.FromUnixTimeSeconds(q));
                    else if (prop.PropertyType == typeof(bool))
                        ret = q > 0 ? true : false;
                    break;

                default:
                    ret = ConvertFromDbType(i.ToString()); // probably not good .... lazy ... etc., 
                    break;
            }
            return ret;
        }
    }
}
