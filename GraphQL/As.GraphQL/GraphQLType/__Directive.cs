using System;
using System.Collections.Generic;

namespace As.GraphQL.GraphQLType
{
    public enum __DirectiveLocation
    {
            QUERY,
          MUTATION,
          FIELD,
          FRAGMENT_DEFINITION,
          FRAGMENT_SPREAD,
          INLINE_FRAGMENT,
    }
    public class __Directive
    {
        public String name { get; set; }
        public String description { get; set; }
        public List<__DirectiveLocation> locations { get; set; }
        public List<__InputValue> args { get; set; }
    }
}
