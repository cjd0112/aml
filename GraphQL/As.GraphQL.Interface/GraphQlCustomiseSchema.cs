using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace As.GraphQL.Interface
{
    public class GraphQlCustomiseSchema 
    {
        GraphQlCommentExtractor ce = new GraphQlCommentExtractor();
        public GraphQlCustomiseSchema()
        {
            
        }
        public virtual bool IncludeInterface(Type source, Type interfaceType)
        {
            return false;
        }

        public virtual String GetDescription(PropertyInfo pi)
        {
            return ce.GetCommentFromXml(pi);
        }

        public virtual String GetDescription(MethodInfo mi)
        {

            return ce.GetCommentFromXml(mi);
        }


        public virtual IEnumerable<Type> GetPossibleTypes(Type interface1)
        {
            return Assembly.GetAssembly(interface1).GetTypes().Where(interface1.IsAssignableFrom).ToArray();
        }

        public virtual bool IncludeProperty(PropertyInfo pi)
        {
            return true;
        }

        public virtual bool IncludeMethod(MethodInfo mi)
        {
            return true;
        }


        public virtual IEnumerable<(string fieldName, string description, Type fieldType,Func<Object,Object> resolver)> AddAdditionalFields(Type type)
        {
            return Enumerable.Empty<(string, string, Type,Func<Object,Object>)>();
        }

        public virtual IEnumerable<(string fieldName, string description, Type fieldType, List<(String,Type)> parameters, Func<Object, Dictionary<string,Object>,Object>  resolver)> AddAdditionalMethods(Type type)
        {
            return Enumerable.Empty<(string, string, Type, List<(String,Type)>, Func<Object, Dictionary<string,object>,Object>)>();
        }



    }
}
