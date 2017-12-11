using System;
using System.Collections.Generic;
using System.Linq;

namespace As.GraphQL.GraphQLType
{
    public class __Field
    {
        public enum FieldTypeEnum
        {
            Method,
            Property
        }

        public __Field(__Type parentType)
        {
            args = new List<__InputValue>();
            this.parentType = parentType;
        }

        public __Type parentType { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public List<__InputValue> args { get; set; }

        public __Type type { get; set; }
        public bool isDeprecated { get; set; }
        public String deprecationReason { get; set; }

        // fields so hidden from GraphQL schema
        public FieldTypeEnum FieldType;
        public Func<Object, Object> ResolveProperty;
        public Func<Object, Dictionary<string, Object>, Object> ResolveMethod;


        internal __InputValue GetInputValue(string nm)
        {
            return args.FirstOrDefault(x => x.name == nm);
        }

        internal void AddInputValue(__InputValue v)
        {
            args.Add(v);
        }

        public override string ToString()
        {
            return name + " " + type.name;
        }
    }


}
