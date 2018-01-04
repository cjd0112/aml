using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using As.Logger;
using Fasterflect;

namespace As.Shared
{
    public class ForeignKey
    {
        public string ChildTableName { get; set; }
        public string ChildFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string ParentTableName { get; set; }
    }
    public class PropertyContainer
    {
        public PropertyInfo pi;
        private MemberGetter getter;
        private MemberSetter setter;
        public int index;
        ForeignKey foreignKey;
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

        public bool IsNumeric()
        {
            return PropertyType == typeof(Int32) || PropertyType == typeof(Int64);
        }

        public bool IsForeignKey => foreignKey != null;

        public ForeignKey GetForeignKey()
        {
            return foreignKey;
        }

        public void SetForeignKey(ForeignKey fk)
        {
            foreignKey = fk;
        }

        public String Name => pi.Name;

        public Type PropertyType => pi.PropertyType;

        public Object GetValue(Object owningObject)
        {
            return getter(owningObject);
        }

        public void SetValue(Object owningObject, Object value)
        {
            setter(owningObject, value);
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



       

    }
}
