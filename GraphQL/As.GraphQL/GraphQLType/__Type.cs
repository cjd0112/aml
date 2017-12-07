using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using As.GraphQL.Interface;

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
            return resolver[t];
        }

        __Type CreateOrGetInputObjectType(Type t)
        {
            if (resolver.ContainsKey(t) == false)
                resolver[t] = new __Type(t, resolver, customiseSchema,true);
            return resolver[t];
        }


        string descriptionFromField(PropertyInfo pi)
        {
            var desc = pi.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null)
                return desc.Description;
            return customiseSchema.GetDescription(pi);
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
            fields = new List<__Field>();
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
                fields.Add(FieldFromProperty(pi.Name, pi.PropertyType,descriptionFromField(pi)));
                if (pi.Name == "fields" && dotNetType == typeof(__Type))
                {
                    fields.Last()
                        .args.Add(new __InputValue("includeDeprecated",
                            CreateOrGetType(typeof(Boolean))));
                }
                if (pi.Name == "enumValues" && dotNetType == typeof(__Type))
                {
                    fields.Last()
                        .args.Add(new __InputValue("includeDeprecated",
                            CreateOrGetType(typeof(Boolean))));
                }

                if (!TypeCheck.IsScalar(pi.PropertyType))
                {
                    foreach (var c in customiseSchema.GetInputValues(pi.Name, pi))
                    {
                        fields.Last().AddInputValue(new __InputValue(c.inputName,CreateOrGetInputObjectType(c.inputType)));
                    }
                }
            }

            // add built-in types
            fields.Add(FieldFromProperty("__typename", typeof(String),"describes type"));

            foreach (var c in customiseSchema.AddAdditionalFields(dotNetType))
            {
                fields.Add(FieldFromProperty(c.fieldName,c.fieldType,c.description));
            }

            description = descriptionFromType(dotNetType);
        }

        void InputObjectType()
        {
            kind = __TypeKind.INPUT_OBJECT;
            name = dotNetType.Name;
            fields = null;
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

        __Field FieldFromProperty(string name, Type t,string desc="")
        {
            var f = new __Field(this);
            f.name = name;
            f.type = CreateOrGetType(t);
            f.description = desc;
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
            enumValues = new List<__EnumValue>();
            foreach (var c in Enum.GetNames(dotNetType))
            {
                enumValues.Add(new __EnumValue(c));
            }
        }

        void ListType()
        {
            name = null;
            kind = __TypeKind.LIST;
            ofType = CreateOrGetType(dotNetType.GenericTypeArguments[0]);
        }
        
        public __TypeKind kind { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public List<__Field> fields { get; set; }
        public List<__Type> interfaces { get; set; }
        public List<__Type> possibleTypes { get; set; }
        public List<__EnumValue> enumValues { get; set; }
        public List<__InputValue> inputFields { get; set; }
        public __Type ofType { get; set; }

        public Type dotNetType;

        // Gets field based on field and ParentType ... 
        public __Field GetField(Predicate<__Field> criteria )
        {
            if (kind == __TypeKind.UNION || kind == __TypeKind.INTERFACE)
            {
                foreach (var c in possibleTypes)
                {
                    var f = c.fields.FirstOrDefault((x) => criteria(x));
                    if (f != null)
                        return f;
                }
                return null;

            }
            else
            {
                if (fields == null)
                    return null;
            }

            return fields.FirstOrDefault(x=>criteria(x));
        }

        public override string ToString()
        {
            return name + " " + kind;
        }
    }
}
