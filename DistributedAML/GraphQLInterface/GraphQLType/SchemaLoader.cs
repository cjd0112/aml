using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLInterface.GraphQLType
{
    public class SchemaLoader<T> where T:class
    {
        private static __SchemaContainer cached = null;
        public static __SchemaContainer GetSchemaContainer()
        {
            if (cached == null)
            {
                var types = new Dictionary<Type, __Type>();
                var schemaContainer = new __SchemaContainer(new __Schema(new __Type(typeof(T), types)));
                var foo =
                    new[]
                        {
                            typeof(T),
                            typeof(__SchemaContainer)
                        }.Select(x => new __Type(x, types))
                        .ToArray();
                schemaContainer.__schema.types = types.Values.Where(SchemaTypeFilter).ToList();
                cached = schemaContainer;
            }
            return cached;
        }

        static bool SchemaTypeFilter(__Type t)
        {
            if (t.kind == __TypeKind.SCALAR || t.kind == __TypeKind.LIST)
                return false;
            return true;
        }     
    }
}
