using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;

namespace As.Shared
{
    public class TypeContainer
    {
        public IEnumerable<PropertyContainer> Properties { get; set; }

        public Type UnderlyingType { get; set; }

        public String Name => UnderlyingType.Name;

        public PropertyContainer GetProperty(String name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        private ConstructorInvoker ci;

        private bool setFirstPropertyPrimaryKey = false;
        private bool includeReadOnlyProperties = false;
        private bool includeInheritedProperties = false;

        public Object CreateInstance()
        {
            return ci();
        }
        public T CreateInstance<T>()
        {
            return (T)ci();
        }

        public TypeContainer(Type t)
        {
            UnderlyingType = t;
        }

        public TypeContainer AddProperties(Predicate<PropertyInfo> property)
        {
            List<PropertyContainer> foo = new List<PropertyContainer>();
            int cnt = 0;

            BindingFlags flags;

            if (includeInheritedProperties)
                flags =  BindingFlags.Public | BindingFlags.Instance;
            else
                flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            
            foreach (var c in UnderlyingType.GetProperties(flags).Where(x=>property(x)))
            {
                if (c.IsWritable() || includeReadOnlyProperties)
                {
                    if (setFirstPropertyPrimaryKey && cnt == 0)
                        foo.Add(new PropertyContainer(c, cnt++, true));
                    else
                        foo.Add(new PropertyContainer(c, cnt++, false));
                }
            }
            Properties = foo;
            return this;
        }

        public TypeContainer IncludeReadOnlyProperties()
        {
            includeReadOnlyProperties = true;
            return this;
        }

        public TypeContainer FirstPropertyIsPrimaryKey()
        {
            setFirstPropertyPrimaryKey = true;
            return this;
        }

        public TypeContainer IncludeInheritedProperties()
        {
            includeInheritedProperties = true;
            return this;
        }

        public TypeContainer SetDelegateForCreateInstance()
        {
            ci = UnderlyingType.DelegateForCreateInstance();
            return this;
        }

        public static IEnumerable<TypeContainer> Initialize(IEnumerable<Type> types)
        {
            lock (typeContainers)
            {
                foreach (var c in types)
                {
                    if (typeContainers.ContainsKey(c) == false)
                    {
                        typeContainers[c] = new TypeContainer(c);
                    }

                    yield return typeContainers[c];
                }

            }
        }
     
        public static Dictionary<Type, TypeContainer> typeContainers = new Dictionary<Type, TypeContainer>();

        public static TypeContainer GetTypeContainer(Type t)
        {
            lock (typeContainers)
            {
                if (typeContainers.ContainsKey(t))
                    return typeContainers[t];
                throw new Exception($"Could not find type - {t.Name} in list of type containers - need to include it in call to 'Initialize'...");
            }
        }
    }
}
