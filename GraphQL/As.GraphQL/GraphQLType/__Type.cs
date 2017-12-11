using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using As.GraphQL.Interface;
using Fasterflect;

namespace As.GraphQL.GraphQLType
{
    public enum __TypeKind
    {
      SCALAR,
      OBJECT,
      INTERFACE,
      UNION,
      ENUM,
      INPUT_OBJECT,
      LIST,
      NON_NULL
    }
    public class __Type
    {
        public __TypeKind kind { get; set; }
        public String name { get; set; }
        public String description { get; set; }

        List<__Field> fields_underlying { get; set; }

        public List<__Field> fields(bool includeDeprecated = true)
        {
            return fields_underlying;
        }

        public List<__Type> interfaces { get; set; }
        public List<__Type> possibleTypes { get; set; }
        List<__EnumValue> enumValues_underlying { get; set; }

        public List<__EnumValue> enumValues(bool includeDeprecated = true)
        {
            return enumValues_underlying;
        }


        public List<__InputValue> inputFields { get; set; }
        public __Type ofType { get; set; }

        internal Type dotNetType;

        public __Type()
        {
            
        }

        private Dictionary<Type, __Type> resolver;
        private GraphQlCustomiseSchema customiseSchema;
        public __Type(Type t,Dictionary<Type,__Type> resolver, GraphQlCustomiseSchema customise,bool isInputType=false)
        {
            this.resolver = resolver;
            this.customiseSchema = customise;
            dotNetType = t;
            
            if (resolver.ContainsKey(t) == false)
                resolver[t] = this;

            if (TypeCheck.IsNumeric(t) || TypeCheck.IsString(t) || TypeCheck.IsDateTime(t) || TypeCheck.IsBoolean(t))
            {
                ScalarType();
            }
            else if (TypeCheck.IsEnum(t))
            {
                EnumType();
            }
            else if (TypeCheck.IsEnumerableType(t))
            {
                ListType();
            }
            else if (TypeCheck.IsClass(t) && isInputType)
            {
                 InputObjectType();                
            }
            else if (TypeCheck.IsClass(t))
            {
                ObjectOrInterfaceType(__TypeKind.OBJECT);
            }
            else if (t.IsInterface)
            {
                ObjectOrInterfaceType(__TypeKind.INTERFACE);
            }
            else if (TypeCheck.IsValueType(t))
            {
               throw new Exception($"Unexpected value type = {t.Name}");
            }
            else
            {
                throw new Exception($"Unexpected type = {t.Name}");
            }
        }

        __Type CreateOrGetType(Type t)
        {
            if (resolver.ContainsKey(t) == false)
                resolver[t] = new __Type(t, resolver, customiseSchema);
            var ret = resolver[t];
            if (ret.kind == __TypeKind.INPUT_OBJECT)
                throw new Exception(
                    $"You cannot mix input and output object types in a schema - {t.Name} already appears as INPUT_OBJECT type");
            return ret;
        }

        __Type CreateOrGetInputObjectType(Type t)
        {
            if (resolver.ContainsKey(t) == false)
                resolver[t] = new __Type(t, resolver, customiseSchema,true);

            var ret = resolver[t];
            if (ret.kind == __TypeKind.OBJECT)
                throw new Exception(
                    $"You cannot mix input and output object types in a schema - {t.Name} already appears as OBJECT type");

            return resolver[t];
        }


        string descriptionFromField(PropertyInfo pi)
        {
            var desc = pi.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null)
                return desc.Description;
            return customiseSchema.GetDescription(pi);
        }

        string descriptionFromMethod(MethodInfo mi)
        {
            var desc = mi.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null)
                return desc.Description;
            return customiseSchema.GetDescription(mi);
        }

        string descriptionFromType(Type t)
        {
            var z = t.GetCustomAttribute<DescriptionAttribute>();
            if (z != null)
                return z.Description;
            return "";
        }

        void ObjectOrInterfaceType(__TypeKind typeKind)
        {
            if (typeKind != __TypeKind.INTERFACE && typeKind != __TypeKind.OBJECT)
                throw new Exception("Expected only 'interface' or 'object' kinds");
            kind = typeKind;
            name = dotNetType.Name;
            fields_underlying = new List<__Field>();
            if (kind == __TypeKind.OBJECT)
            {
                interfaces = new List<__Type>();

                foreach (var c in dotNetType.GetTypeInfo().ImplementedInterfaces)
                {
                    if (customiseSchema.IncludeInterface(dotNetType,c))
                    {
                        interfaces.Add(CreateOrGetType(c));
                    }
                }
            }
            else
            {
                possibleTypes = new List<__Type>();

                foreach (var c in customiseSchema.GetPossibleTypes(dotNetType))
                {
                    possibleTypes.Add(CreateOrGetType(c));
                }
            }

            foreach (
            PropertyInfo pi in dotNetType.GetProperties().Where(x=>customiseSchema.IncludeProperty(x)))
            {
                fields_underlying.Add(FieldFromProperty(pi.Name, pi.PropertyType,pi.DeclaringType,descriptionFromField(pi)));
            }


            foreach (MethodInfo mi in dotNetType
                .GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                .Where(x => customiseSchema.IncludeMethod(x)))
            {
                if (!mi.IsSpecialName)
                {
                    fields_underlying.Add(FieldFromMethod(mi.Name, mi.ReturnType, mi.DeclaringType, mi.GetParameters().Select(x=>(x.Name,x.ParameterType)).ToList(), descriptionFromMethod(mi)));
                }
            }

            // add built-in types
            fields_underlying.Add(FieldFromProperty("__typename", typeof(String), dotNetType,  "describes type",(x)=>dotNetType.Name));

            foreach (var c in customiseSchema.AddAdditionalFields(dotNetType))
            {
                fields_underlying.Add(FieldFromProperty(c.fieldName,c.fieldType,dotNetType,c.description,c.resolver));
            }

            foreach (var c in customiseSchema.AddAdditionalMethods(dotNetType))
            {
                fields_underlying.Add(FieldFromMethod(c.fieldName, c.fieldType, dotNetType, c.parameters, c.description,c.resolver));
            }


            description = descriptionFromType(dotNetType);
        }

        void InputObjectType()
        {
            kind = __TypeKind.INPUT_OBJECT;
            name = dotNetType.Name;
            fields_underlying = null;
            interfaces = null;
            inputFields = new List<__InputValue>();
            foreach (
                PropertyInfo pi in dotNetType.GetProperties())
            {
                inputFields.Add(new __InputValue(pi.Name, CreateOrGetType(pi.PropertyType)));
            }
        }

        /*
        void UnionType()
        {
            kind = __TypeKind.UNION;
            name = dotNetType.Name;
            possibleTypes = new List<__Type>();
            var types = typeUniverse.Where((x) => dotNetType.IsAssignableFrom(x) && x != dotNetType && !x.IsInterface ).ToArray();

            foreach (var c in types)
            {
                possibleTypes.Add(CreateOrGetType(c));
            }

            description = descriptionFromType(dotNetType);
        }
        */

        __Field FieldFromProperty(string name, Type t, Type declaringType,string desc="",Func<Object,Object> propertyResolver=null)
        {
            var f = new __Field(this);
            f.name = name;
            f.type = CreateOrGetType(t);
            f.description = desc;
            f.ResolveProperty = propertyResolver;
            f.FieldType = __Field.FieldTypeEnum.Property;

            if (f.ResolveProperty == null)
            {
                if (typeof(ISupportGetValue).IsAssignableFrom(declaringType))
                {
                    f.ResolveProperty = (Object foo) => ((ISupportGetValue) foo).GetValue(f.name);
                }
                else
                {
                    var propertyDelegate = declaringType.DelegateForGetPropertyValue(name);
                    f.ResolveProperty = (Object foo) => propertyDelegate(foo);
                }
            }

            return f;
        }

        __Field FieldFromMethod(string name, Type t, Type declaringType, List<(string paramName,Type paramType)> params1, string desc = "",Func<Object,Dictionary<string,Object>,Object> methodResolver=null)
        {
            var f = new __Field(this);
            f.name = name;
            f.type = CreateOrGetType(t);
            f.description = desc;
            f.args = new List<__InputValue>();
            f.FieldType = __Field.FieldTypeEnum.Method;
            foreach (var c in params1)
            {                
                f.AddInputValue(new __InputValue(c.paramName, CreateOrGetInputObjectType(c.paramType)));
            }

            f.ResolveMethod = methodResolver;

            if (f.ResolveMethod == null)
            {
                var methodDelegate = declaringType.DelegateForCallMethod(name, params1.Select(x => x.paramType).ToArray());
                String[] names = params1.Select(x => x.paramName).ToArray();
                f.ResolveMethod = (object o, Dictionary<string, Object> args) =>
                {
                    int cnt = 0;
                    Object[] paramArray = new object[params1.Count()];
                    foreach (var c in names)
                    {
                        if (args.TryGetValue(c, out var res))
                        {
                            paramArray[cnt++] = res;
                        }
                        else
                        {
                            paramArray[cnt++] = null;
                        }

                    }
                    return methodDelegate(o, paramArray);
                };
            }

            return f;
        }


        void ScalarType()
        {
            kind = __TypeKind.SCALAR;
            if (TypeCheck.IsNumeric(dotNetType))
            {
                if (dotNetType == typeof(float))
                {
                    name = "Float";
                }
                else if (dotNetType == typeof(int) || dotNetType == typeof(uint) || dotNetType == typeof(short) || dotNetType == typeof(ushort))
                {
                    name = "Int";
                }
                else
                {
                    name = "String";
                }
            }
            else if (TypeCheck.IsString(dotNetType))
            {
                name = "String";
            }
            else if (TypeCheck.IsBoolean(dotNetType))
            {
                name = "Boolean";
            }
            else if (TypeCheck.IsDateTime(dotNetType))
            {
                name = "String";
            }
            else if (dotNetType.Name == "IndexRelation" || dotNetType.Name == "IndexReference")
            {
                name = "ID";
            }
            else
            {
                throw new Exception($"Unexpected scalar type - {dotNetType.Name}");
            }
        }

        void EnumType()
        {
            kind = __TypeKind.ENUM;
            name = dotNetType.Name;
            enumValues_underlying = new List<__EnumValue>();
            foreach (var c in Enum.GetNames(dotNetType))
            {
                enumValues_underlying.Add(new __EnumValue(c));
            }
        }

        void ListType()
        {
            name = null;
            kind = __TypeKind.LIST;
            ofType = CreateOrGetType(dotNetType.GenericTypeArguments[0]);
        }
        
     

        // Gets field based on field and ParentType ... 
        internal __Field GetField(Predicate<__Field> criteria )
        {
            if (kind == __TypeKind.UNION || kind == __TypeKind.INTERFACE)
            {
                foreach (var c in possibleTypes)
                {
                    var f = c.fields_underlying.FirstOrDefault((x) => criteria(x));
                    if (f != null)
                        return f;
                }
                return null;

            }
            else
            {
                if (fields_underlying == null)
                    return null;
            }

            return fields_underlying.FirstOrDefault(x=>criteria(x));
        }

        public override string ToString()
        {
            return name + " " + kind;
        }
    }
}
