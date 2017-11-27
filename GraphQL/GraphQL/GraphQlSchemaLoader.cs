using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.GraphQLType;
using GraphQL.Interface;

namespace GraphQL
{
    public class GraphQlSchemaLoader
    {
        private static Dictionary<Type, __SchemaContainer> cached = new Dictionary<Type, __SchemaContainer>();

        public static __SchemaContainer GetSchema(Type root)
        {
            if (cached.ContainsKey(root) == false)
            {
                return null;
            }
            else
            {
                return cached[root];
            }
        }

        public static __SchemaContainer InitializeSchema(Type root, GraphQlCustomiseSchema custom)
        {
            if (cached.ContainsKey(root) == false)
            {
                var types = new Dictionary<Type, __Type>();
                var schemaContainer = new __SchemaContainer(new __Schema(new __Type(root, types,custom)));
                var foo =
                    new[]
                        {
                            root,
                            typeof(__SchemaContainer)
                        }.Select(x => new __Type(x, types, custom))
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
