using System;

namespace As.GraphQL.GraphQLType
{
    public class __EnumValue
    {
        public __EnumValue()
        {
            
        }

        public __EnumValue(string n)
        {
            name = n;
        }
        public String name { get; set; }
        public String description { get; set; }
        public bool isDeprecated { get; set; }
        public String deprecationReason { get; set; }
    }
}
