using System;
using System.Collections.Generic;
using System.Linq;

namespace As.GraphQL.GraphQLType
{
    public class __Schema
    {
        public __Schema()
        {
            
        }
        public __Schema(__Type queryType)
        {
            this.queryType = queryType;
        }

        public __Schema(__Type queryType,__Type mutationType)
        {
            this.queryType = queryType;
            this.mutationType = mutationType;
        }


        public List<__Type> types { get; set; }
        public __Type queryType { get; set; }
        public __Type mutationType { get; set; }
        public List<__Directive> directives { get; set; }
        public __Type subscriptionType { get; set; }

        public __Type GetType(String name)
        {
            return types.FirstOrDefault(x => x.name == name);
        }
    }
}
