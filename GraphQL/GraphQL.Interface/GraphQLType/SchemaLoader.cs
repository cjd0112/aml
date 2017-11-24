using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQLInterface.GraphQLType
{
    public class SchemaLoader
    {
        private static Dictionary<Type,__SchemaContainer> cached = new Dictionary<Type, __SchemaContainer>();
        public static __SchemaContainer GetSchemaContainer(Type root,IEnumerable<Type> typeUniverse,Func<PropertyInfo,bool> includeProperty)
        {
            if (cached.ContainsKey(root) == false)
            {
                var types = new Dictionary<Type, __Type>();
                var schemaContainer = new __SchemaContainer(new __Schema(new __Type(root, types,typeUniverse,includeProperty)));
                var foo =
                    new[]
                        {
                            root,
                            typeof(__SchemaContainer)
                        }.Select(x => new __Type(x, types,typeUniverse,includeProperty))
                        .ToArray();
                schemaContainer.__schema.types = types.Values.Where(SchemaTypeFilter).ToList();
                cached[root] = schemaContainer;
            }
            return cached[root];
        }

        static bool SchemaTypeFilter(__Type t)
        {
            if (t.kind == __TypeKind.SCALAR || t.kind == __TypeKind.LIST)
                return false;
            return true;
        }     
    }
}
