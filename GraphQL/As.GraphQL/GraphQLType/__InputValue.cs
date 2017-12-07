using System;

namespace As.GraphQL.GraphQLType
{
    public class __InputValue
    {
        public __InputValue()
        {
            
        }

        public __InputValue(string name, __Type type)
        {
            this.name = name;
            this.type = type;
        }
        public String name { get; set; }
        public String description { get; set; }
        public __Type type { get; set; }
        public String defaultValue { get; set; }
    }
}
