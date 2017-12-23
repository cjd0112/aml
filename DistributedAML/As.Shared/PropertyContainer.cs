using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using As.Logger;
using Fasterflect;

namespace As.Shared
{
    public class ForeignKey
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
    }
    public class PropertyContainer
    {
        public PropertyInfo pi;
        public MemberGetter getter;
        public MemberSetter setter;
        public int index;
        public ForeignKey foreignKey;
        public bool IsPrimaryKey = false;

        public PropertyContainer(PropertyInfo pi,int index = 0,bool isPrimaryKey=false)
        {
            if (pi.GetMethod == null)
                throw new Exception($"{pi.Name} - has not get method");
            getter = pi.DelegateForGetPropertyValue();
            if (pi.SetMethod != null)
            {
                setter = pi.DelegateForSetPropertyValue();
            }
            else
            {
                L.Trace($"Property - {pi.Name} in type - {pi.DeclaringType.Name} has no setter ... ");
            }
            this.index = index;
            this.pi = pi;
            this.IsPrimaryKey = isPrimaryKey;
        }

        public String Name => pi.Name;

        public Type PropertyType => pi.PropertyType;

        public Object GetValue(Object owningObject)
        {
            return getter(owningObject);
        }

        public void SetValue(Object owningObject, Object value)
        {
            setter(owningObject, Convert(value));
        }

        public bool Display()
        {
            var da = pi.GetCustomAttribute<DisplayAttribute>();
            return da == null || da.AutoGenerateField == true;
        }

        public String DisplayName()
        {
            var da = pi.GetCustomAttribute<DisplayAttribute>();
            if (da != null && !String.IsNullOrEmpty(da.Name))
                return da.Name;

            return Name.ConvertCamelCase();
        }



        // probably a bit more to do here
        public Object Convert(Object i)
        {
            if (i == null)
            {
                if (pi.PropertyType == typeof(String))
                    return "";
                else
                    return 0;
            }

            Object ret = null;
            switch (i)
            {
                case String s:
                    if (pi.PropertyType == typeof(String))
                        ret = s;
                    else if (pi.PropertyType == typeof(Boolean))
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
                    else if (pi.PropertyType.IsEnum)
                        ret = Enum.Parse(pi.PropertyType, s);
                    else if (pi.PropertyType == typeof(Int32))
                        ret = System.Convert.ToInt32(s);
                    else if (pi.PropertyType == typeof(Int64))
                        ret = System.Convert.ToInt64(s);
                    else
                        ret = i;
                    break;
                default:
                    ret = Convert(i.ToString()); // probably not good .... lazy ... etc., 
                    break;
            }
            return ret;
        }

    }
}
