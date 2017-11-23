using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace GraphQLInterface.GraphQLType
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
        public __Type(Type t,Dictionary<Type,__Type> resolver)
        {
            this.resolver = resolver;
            dotNetType = t;

            if (resolver.ContainsKey(t) == false)
                resolver[t] = this;

            if (General.IsNumeric(t) || General.IsString(t) || General.IsDateTime(t) || General.IsBoolean(t))
            {
                ScalarType();
            }
            else if (General.IsEnum(t))
            {
                EnumType();
            }
            else if (General.IsListType(t))
            {
                ListType();
            }
            else if (General.IsClass(t))
            {
                var graphQlAttribute = t.GetTypeInfo().GetCustomAttribute<GraphQLAttribute>();
                if (typeof(ISearchObject).GetTypeInfo().IsAssignableFrom(t) || (graphQlAttribute != null && graphQlAttribute.AttributeType == GraphQLAttributeTypeEnum.TreatAsInputObject))
                {
                    InputObjectType();
                }
                else
                {
                    ObjectOrInterfaceType(__TypeKind.OBJECT);
                }
            }
            else if (t.GetTypeInfo().IsInterface)
            {
                if (typeof(IGraphQLInterface).GetTypeInfo().IsAssignableFrom(t))
                {
                    ObjectOrInterfaceType(__TypeKind.INTERFACE);
                }
                else if (typeof(IGraphQLUnionRelation).GetTypeInfo().IsAssignableFrom(t))
                {
                    UnionType();
                }
             
                else
                {
                    throw new Exception($"Currently only interface types supported are derived from 'IUnionRelation'");
                }
            }
            else if (General.IsValueType(t))
            {
                if (t.Name == "IndexRelation" || t.Name == "IndexReference")
                {
                    ScalarType();
                }
                else
                {
                    throw new Exception($"Unexpected value type = {t.Name}");
                }
            }
            else
            {
                throw new Exception($"Unexpected type = {t.Name}");
            }
        }

        string descriptionFromField(PropertyInfo pi)
        {
            var desc = pi.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null)
                return desc.Description;
            return "";
        }

        string descriptionFromType(Type t)
        {
            var z = t.GetTypeInfo().GetCustomAttribute<DescriptionAttribute>();
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
                    if (typeof(IGraphQLInterface).GetTypeInfo().IsAssignableFrom(c))
                    {
                        interfaces.Add(CreateOrGetType(c));
                    }
                }
            }
            else
            {
                possibleTypes = new List<__Type>();

                foreach (var c in SelectTypes.GetClasses(General.GetMainApplicationPath(), (x) => true,
                    (x) => dotNetType.GetTypeInfo().IsAssignableFrom(x) && x != dotNetType))
                {
                    possibleTypes.Add(CreateOrGetType(c));
                }
            }

            foreach (
            PropertyInfo pi in dotNetType.GetTypeInfo().GetProperties())
            {
                if (pi.PropertyType == typeof(IndexReference))
                {
                    // change the IndexRelation type to (e.g.,) List<IRelatedToTransaction> type
                    var g = dotNetType.GetTypeInfo().GetCustomAttribute<GraphQLAttribute>();
                    if (g == null)
                    {
                        fields.Add(FieldFromProperty(pi.Name, pi.PropertyType,descriptionFromField(pi)));
                    }
                    else
                    {
                        if (g.RelationUnionType == null)
                            throw new Exception($"Found class {dotNetType.Name} with null RelationUnionType and GraphQLAttribute");
                        var listUnderlyingType = g.RelationUnionType.GenericTypeArguments[0];
                        if (typeof(IGraphQLUnionRelation).GetTypeInfo().IsAssignableFrom(listUnderlyingType) == false)
                            throw new Exception($"Found class {dotNetType.Name} with IndexReference where its underlying type - {listUnderlyingType.Name} - does not inherit from IGraphQLUnionRelation");

                        fields.Add(FieldFromProperty(pi.Name, g.RelationUnionType,descriptionFromField(pi)));
                        CustomArgumentsFromRelationUnion(g.RelationUnionType, fields.Last());

                    }
                }
                else
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

                    var customArguments = pi.GetCustomAttribute<GraphQLAttribute>();
                    if (customArguments != null)
                    {
                        CustomArguments(customArguments, pi.PropertyType, fields.Last());
                    }
                }
            }

            // add built-in types
            fields.Add(FieldFromProperty("__typename", typeof(String),"describes type"));
            fields.Add(FieldFromProperty("relation", typeof(RelationRoles),"Role this object is playing relative to 'parent' object"));
            fields.Add(FieldFromProperty("relevance", typeof(float),"Indicates the relevance ranking of this object within a query (e.g., fuzzy match)"));
            fields.Add(FieldFromProperty("__all",typeof(String),"shortcut to add all properties"));

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
                PropertyInfo pi in dotNetType.GetTypeInfo().GetProperties())
            {
                inputFields.Add(new __InputValue(pi.Name, CreateOrGetType(pi.PropertyType)));
            }
        }

     



        void UnionType()
        {
            kind = __TypeKind.UNION;
            name = dotNetType.Name;
            possibleTypes = new List<__Type>();

            var types = SelectTypes.GetClassesAndInterfaces(General.GetMainApplicationPath(), (x) => true,
               (x) => dotNetType.GetTypeInfo().IsAssignableFrom(x) && x != dotNetType && !x.GetTypeInfo().IsInterface ).ToArray();

            foreach (var c in types)
            {
                possibleTypes.Add(CreateOrGetType(c));
            }

            description = descriptionFromType(dotNetType);
        }

        __Field FieldFromProperty(string name, Type t,string desc="")
        {
            var f = new __Field(this);
            f.name = name;
            f.type = CreateOrGetType(t);
            f.description = desc;
            return f;
        }

        __Type CreateOrGetType(Type t)
        {
            if (resolver.ContainsKey(t) == false)
                resolver[t] = new __Type(t, resolver);
            return resolver[t];
        }


        void ScalarType()
        {
            kind = __TypeKind.SCALAR;
            if (General.IsNumeric(dotNetType))
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
            else if (General.IsString(dotNetType))
            {
                name = "String";
            }
            else if (General.IsBoolean(dotNetType))
            {
                name = "Boolean";
            }
            else if (General.IsDateTime(dotNetType))
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

        void CustomArguments(GraphQLAttribute attribute,Type sourceType,__Field field)
        {
            if (attribute.AttributeType == GraphQLAttributeTypeEnum.SupportSearchAllFields)
            {
                if (typeof(IList).GetTypeInfo().IsAssignableFrom(sourceType))
                {
                    var underlyingType = sourceType.GenericTypeArguments[0];


                    //field.AddInputValue(new __InputValue(z.Name,CreateOrGetType(z)));
                    foreach (var c in underlyingType.GetTypeInfo().GetProperties())
                    {
                        if (General.IsScalar(c.PropertyType))
                            field.AddInputValue(new __InputValue(c.Name, CreateOrGetType(c.PropertyType)));
                    }
                }
            }
            field.AddInputValue(new __InputValue("Sort", CreateOrGetType(typeof(Sort))));
            field.AddInputValue(new __InputValue("Range",CreateOrGetType(typeof(Range))));
            field.AddInputValue(new __InputValue("Table",CreateOrGetType(typeof(String))));
        }

        void CustomArgumentsFromRelationUnion(Type sourceType, __Field field)
        {
            if (typeof(IList).GetTypeInfo().IsAssignableFrom(sourceType))
            {
                var underlyingType = sourceType.GenericTypeArguments[0];

                var types = SelectTypes.GetClasses(General.GetMainApplicationPath(), (x) => true,
                   (x) => underlyingType.GetTypeInfo().IsAssignableFrom(x)).ToArray();
                foreach (var t in types)
                {
                    foreach (var c in t.GetTypeInfo().GetProperties())
                    {
                        if (General.IsScalar(c.PropertyType))
                            field.AddInputValue(new __InputValue(t.Name + "__" + c.Name, CreateOrGetType(c.PropertyType)));
                    }
                }
            }
            field.AddInputValue(new __InputValue("Range", CreateOrGetType(typeof(Range))));
            field.AddInputValue(new __InputValue("relation",CreateOrGetType(typeof(List<RelationRoles>))));
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
