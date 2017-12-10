using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using As.GraphQL.GraphQLType;
using As.GraphQL.Interface;
using As.Logger;

namespace As.GraphQL
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
                var queryAttribute = root.GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttribute(typeof(GraphQlTopLevelQueryAttribute)) != null);

                if (queryAttribute == null)
                    throw new Exception(
                        $"Top Level query - {root.Name} - must contain a public property with '[GRaphQlQuery]' as attribute - to indicate top level query ... if mutations are required then must include [GraphQlMutations] attribute on a different property");

                var queryType = root.GetProperties().Where(x => x.GetCustomAttribute<GraphQlTopLevelQueryAttribute>() != null)
                    .Select(x => x.PropertyType).First();

                var mutationAttribute = root.GetProperties()
                    .FirstOrDefault(x => x.GetCustomAttribute(typeof(GraphQlMutationsAttribute)) != null);

                Type mutationType = null;

                if (mutationAttribute == null)
                    L.Trace($"[GraphQlMutation] attribute on {root.Name} not found ... continuing");
                else
                {
                    mutationType = root.GetProperties()
                        .Where(x => x.GetCustomAttribute<GraphQlMutationsAttribute>() != null)
                        .Select(x => x.PropertyType).First();
                }


                var types = new Dictionary<Type, __Type>();

                var __queryType = new __Type(queryType, types, custom);

                __SchemaContainer schemaContainer = null;
                if (mutationType == null)
                {
                    schemaContainer = new __SchemaContainer(new __Schema(__queryType));
                }
                else
                {
                    var __mutationType = new __Type(mutationType, types, custom);
                    schemaContainer = new __SchemaContainer(new __Schema(__queryType,__mutationType));
                }

                var foo =
                    new[]
                        {
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
