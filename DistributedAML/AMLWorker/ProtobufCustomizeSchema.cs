using System;
using System.Collections.Generic;
using System.Reflection;
using As.GraphQL.Interface;
using Google.Protobuf;

namespace AMLWorker
{
    public class ProtobufCustomizeSchema : GraphQlCustomiseSchema
    {
        public override bool IncludeProperty(PropertyInfo pi)
        {
            if (typeof(IMessage).IsAssignableFrom(pi.DeclaringType))
            {
                if (pi.PropertyType.Name.StartsWith("MessageParser") ||
                    pi.PropertyType.Name == "MessageDescriptor")
                    return false;
            }

            return true;
        }

        public override bool IncludeMethod(MethodInfo mi)
        {
            // don't include any IMessage functions 
            if (typeof(IMessage).IsAssignableFrom(mi.DeclaringType))
                return false;
            return true;
        }

        public override IEnumerable<(string fieldName, string description, Type fieldType,Func<Object,Object> resolver)> AddAdditionalFields(Type type)
        {
            return base.AddAdditionalFields(type);
        }

        public override IEnumerable<(string fieldName, string description, Type fieldType, List<(String,Type)> parameters, Func<Object, Dictionary<string,Object>, Object> resolver)> AddAdditionalMethods(Type type)
        {
            return base.AddAdditionalMethods(type);
        }

    }
}