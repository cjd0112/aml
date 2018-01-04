using System;
using System.Collections.Generic;
using System.Text;

namespace As.Shared
{
    public class ProtobufProperties
    {
        private PropertyContainer prop = null;
        public ProtobufProperties(PropertyContainer c)
        {
            this.prop = c;
        }

        public PropertyContainer Underlying => prop;

        public String Name => prop.Name;
        public System.Type PropertyType => prop.PropertyType;

        public Object GetValue(object owningObject)
        {
            return ConvertToProtobufType(prop.GetValue(owningObject));
        }

        public void SetValue(object owningObject, object value)
        {
            prop.SetValue(owningObject, ConvertFromProtobufType(value));
        }

        private Object ConvertToProtobufType(Object i)
        {
            if (i == null)
            {
                if (PropertyType == typeof(String))
                    return "";
            }

            return i;
        }

        // probably a bit more to do here
        private Object ConvertFromProtobufType(Object i)
        {
            return i;
        }

    }
}
