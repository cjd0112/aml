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

        public Object CreateInstance()
        {
            return ci();
        }
        public T CreateInstance<T>()
        {
            return (T)ci();
        }

        public TypeContainer(Type t,Predicate<PropertyInfo> filter = null)
        {
            UnderlyingType = t;

            ci = t.DelegateForCreateInstance();

            var foo = new List<PropertyContainer>();

            int cnt = 0;
            foreach (var c in t.GetProperties().Where(x=>filter == null || filter(x)))
            {
                foo.Add(new PropertyContainer(c, cnt++));
            }

            Properties = foo;

        }

        public static Dictionary<Type, TypeContainer> typeContainers = new Dictionary<Type, TypeContainer>();

        public static TypeContainer GetTypeContainer(Type t,Predicate<PropertyInfo> filter=null)
        {
            lock (typeContainers)
            {
                if (typeContainers.TryGetValue(t, out var container))
                    return container;
                return typeContainers[t] = new TypeContainer(t,filter);
            }
        }
    }
}
