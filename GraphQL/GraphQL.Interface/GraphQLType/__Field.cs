using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLInterface.GraphQLType
{
    public class __Field
    {
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

        // indicates is not explicitly set on .dotNetType like, e.g., List<AsCustomer> on AsTransaction
        public bool IsVirtualField { get; set; }

        public __InputValue GetInputValue(string nm)
        {
            return args.FirstOrDefault(x => x.name == nm);
        }

        public void AddInputValue(__InputValue v)
        {
            args.Add(v);
        }

        public override string ToString()
        {
            return name + " " + type.name;
        }
    }


}
