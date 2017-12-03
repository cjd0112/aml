using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace As.GraphQL.Interface
{
    public class GraphQlCustomiseSchema 
    {
        public virtual IEnumerable<Type> GetInputTypes(Type source)
        {
            yield break;
        }

        public virtual bool IncludeInterface(Type source, Type interfaceType)
        {
            return false;
        }

        public virtual String GetDescription(PropertyInfo pi)
        {
            return "";
        }

        public virtual IEnumerable<Type> GetPossibleTypes(Type interface1)
        {
            return Assembly.GetAssembly(interface1).GetTypes().Where(interface1.IsAssignableFrom).ToArray();
        }

        public virtual bool IncludeProperty(PropertyInfo pi)
        {
            return true;
        }

        protected virtual bool UseAllSearchPropertiesForEnumerables { get; set; }


        protected IEnumerable<(string propertyName, Type propertyType)> GetAllSearchableValues(Type type)
        {
            foreach (var c in type.GetProperties())
            {
                if (TypeCheck.IsScalar(c.PropertyType))
                    yield return (c.Name, c.PropertyType);
            }
        }

        public virtual IEnumerable<(string inputName, Type inputType)> GetInputValues(string fieldName, PropertyInfo pi)
        {
            if (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(String))
            {
                return GetAllSearchableValues(pi.PropertyType.GenericTypeArguments[0]);
            }
            return Enumerable.Empty<(string, Type)>();
        }

        public virtual IEnumerable<(string fieldName, string description, Type fieldType)> AddAdditionalFields(Type type)
        {
            return Enumerable.Empty<(string, string, Type)>();
        }


    }
}
