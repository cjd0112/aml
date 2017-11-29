using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using GraphQL.GraphQLType;
using GraphQL.Interface;
using GraphQL.Utilities;

namespace GraphQL
{
    public class GraphQlMainExecution : GraphQlMainBase 
    {
        public bool IsSchemaQuery { get; set; }

        private Object TopLevelObject;

        public Func<String, __Type> GetTypeFunc;

        private IGraphQlOutput output;

        private IGraphQlDatabase db;

        public GraphQlMainExecution(IGraphQlDocument document, __SchemaContainer schema, IGraphQlOutput output, Object topLevelObject, IGraphQlDatabase db = null,  string operationName = "") 
            : base(document, schema)
        {
            this.TopLevelObject = topLevelObject;
            this.output = output;
            this.db = db;
            var queryType = schema.__schema.queryType;

            GetTypeFunc = schema.GetType;

            if (queryType.kind != __TypeKind.OBJECT)
                Error($"Top level query type must be an object not - {queryType.kind}", documentContext);

            var operation = GetTopLevelOperation(operationName);

            var variables = new Dictionary<string, Object>();

            output.PushObject("data");

            ExecuteSelectionSet(new ModifiableSelectionSet(operation.selectionSet()), queryType, topLevelObject,
                variables);

            output.Pop();

        }

        void ExecuteSelectionSet(ModifiableSelectionSet ss, __Type objectType, Object objectValue,
            Dictionary<string, Object> variableValues)
        {
            var visitedFragments = new Dictionary<string, GraphQLParser.FragmentDefinitionContext>();
            var groupedFieldSet = CollectFields(objectType, ss, variableValues, visitedFragments);
            var resultMap = new OrderedDictionary<string, object>();

            foreach (string responseKey in groupedFieldSet.Keys)
            {
                var fields = groupedFieldSet[responseKey] as List<GraphQLParser.FieldContext>;
                var fieldName = fields.First().fieldName().GetText();

                var field = objectType.GetField((x) => x.name == fieldName);

                if (field == null)
                {
                    if (fieldName == "__schema")
                    {
                        IsSchemaQuery = true;
                        field = schema.GetType("__SchemaContainer").GetField((x) => x.name == "__schema");
                        objectValue = schema;
                        objectType = schema.GetType("__SchemaContainer");
                    }
                    else
                    {
                        continue;
                    }
                }

                var fieldType = field.type;


                if (objectType.kind == __TypeKind.UNION)
                {
                    if (field.parentType.dotNetType != objectValue.GetType())
                        continue;

                    ExecuteField(fields.First(), field.parentType, objectValue, fieldType, fields, variableValues,
                        visitedFragments);

                }
                else
                {
                    ExecuteField(fields.First(), objectType, objectValue, fieldType, fields, variableValues,
                        visitedFragments);

                }

            }
        }

        public Dictionary<__Type, GraphQLParser.SelectionSetContext> GetTypedSelectionSetContextFromUnionType(
            __Type unionType, GraphQLParser.SelectionSetContext ss)
        {
            Dictionary<__Type, GraphQLParser.SelectionSetContext> ret =
                new Dictionary<__Type, GraphQLParser.SelectionSetContext>();

            foreach (var s in ss.selection())
            {
                if (s.field() != null)
                    Error($"Not expecting field under Union type - {unionType.name}", ss);
                else if (s.fragmentSpread() != null)
                {
                    var fragmentSpreadName = s.fragmentSpread().fragmentName().GetText();

                    var fragment = GetFragments(x => x.fragmentName().GetText() == fragmentSpreadName).FirstOrDefault();
                    if (fragment == null)
                        continue;

                    var fragmentTypeName = fragment.typeCondition().typeName().GetText();

                    var fragmentType = schema.GetType(fragmentTypeName);

                    if (!DoesFragmentTypeApply(unionType, fragmentType, fragment))
                        continue;

                    if (fragmentType.kind == __TypeKind.UNION)
                    {
                        var z = GetTypedSelectionSetContextFromUnionType(fragmentType, fragment.selectionSet());
                        foreach (var c in z.Keys)
                        {
                            ret[c] = z[c];
                        }
                    }
                    else
                    {
                        ret[fragmentType] = fragment.selectionSet();
                    }

                }
                else if (s.inlineFragment() != null)
                {
                    var fragmentType = schema.GetType(s.inlineFragment().typeCondition().GetText());
                    if (fragmentType != null && !DoesFragmentTypeApply(unionType, fragmentType, s.inlineFragment()))
                    {
                        continue;
                    }
                    ret[fragmentType] = s.inlineFragment().selectionSet();
                }
            }
            return ret;
        }

        public OrderedDictionary<string, List<GraphQLParser.FieldContext>> CollectFields(__Type objectType,
            ModifiableSelectionSet ss,
            Dictionary<string, Object> variableValues,
            Dictionary<string, GraphQLParser.FragmentDefinitionContext> visitedFragments)
        {
            var groupedFields = new OrderedDictionary<string, List<GraphQLParser.FieldContext>>();
            foreach (var s in ss.selections)
            {
                if (s.field() != null)
                {
                    if (skipDirective(s.field()))
                        continue;

                    if (!includeDirective(s.field()))
                        continue;

                    var responseKey = s.field().fieldName().GetText();
                    if (groupedFields.ContainsKey(responseKey) == false)
                        groupedFields[responseKey] = new List<GraphQLParser.FieldContext>();
                    groupedFields[responseKey].Add(s.field());
                }
                else if (s.fragmentSpread() != null)
                {
                    var fragmentSpreadName = s.fragmentSpread().fragmentName().GetText();
                    if (visitedFragments.ContainsKey(fragmentSpreadName))
                        continue;

                    var fragment = GetFragments(x => x.fragmentName().GetText() == fragmentSpreadName).FirstOrDefault();
                    if (fragment == null)
                        continue;

                    var fragmentTypeName = fragment.typeCondition().typeName().GetText();

                    var fragmentType = schema.GetType(fragmentTypeName);

                    if (!DoesFragmentTypeApply(objectType, fragmentType, fragment))
                        continue;

                    var fragmentSelectionSet = new ModifiableSelectionSet(fragment.selectionSet());
                    var fragmentGroupedFieldSet = CollectFields(objectType, fragmentSelectionSet, variableValues,
                        visitedFragments);

                    foreach (string responseKey in fragmentGroupedFieldSet.Keys)
                    {
                        if (groupedFields.ContainsKey(responseKey) == false)
                            groupedFields[responseKey] = new List<GraphQLParser.FieldContext>();
                        groupedFields[responseKey].AddRange(fragmentGroupedFieldSet[responseKey]);
                    }
                }
                else if (s.inlineFragment() != null)
                {
                    var fragmentType = schema.GetType(s.inlineFragment().typeCondition().GetText());
                    if (fragmentType != null && !DoesFragmentTypeApply(objectType, fragmentType, s.inlineFragment()))
                    {
                        continue;
                    }
                    var fragmentSelectionSet = new ModifiableSelectionSet(s.inlineFragment().selectionSet());
                    var fragmentGroupedFieldSet = CollectFields(objectType, fragmentSelectionSet, variableValues,
                        visitedFragments);

                    foreach (string responseKey in fragmentGroupedFieldSet.Keys)
                    {
                        if (groupedFields.ContainsKey(responseKey) == false)
                            groupedFields[responseKey] = new List<GraphQLParser.FieldContext>();
                        groupedFields[responseKey].AddRange(fragmentGroupedFieldSet[responseKey]);
                    }
                }
            }
            return groupedFields;
        }

        bool DoesFragmentTypeApply(__Type objectType, __Type fragmentType, ParserRuleContext ctx)
        {
            if (fragmentType.kind == __TypeKind.OBJECT || fragmentType.kind == __TypeKind.INTERFACE)
            {
                if (objectType.kind == __TypeKind.UNION || objectType.kind == __TypeKind.INTERFACE)
                {
                    return objectType.possibleTypes.Contains(fragmentType);
                }
                else
                {
                    if (fragmentType.kind != objectType.kind)
                        return false;
                }
            }
            else if (fragmentType.kind == __TypeKind.UNION)
            {
                if (objectType != fragmentType)
                    return false;
            }
            else
            {
                Error($"Unexpected fragment type found - {fragmentType.kind}", ctx);
            }
            return true;
        }


        bool skipDirective(GraphQLParser.FieldContext fc)
        {
            // TODO: process skip directive ... 
            return false;
        }

        bool includeDirective(GraphQLParser.FieldContext fc)
        {
            // TODO: process include directive ..
            return true;
        }

        void ExecuteField(GraphQLParser.FieldContext parentField, __Type objectType, Object objectValue,
            __Type fieldType,
            List<GraphQLParser.FieldContext> fields,
            Dictionary<string, Object> variableValues,
            Dictionary<string, GraphQLParser.FragmentDefinitionContext> visitedFragments)
        {
            var field = fields.First();

            var argumentValues = CoerceArgumentValues(objectType, field, variableValues);


            if (objectValue == TopLevelObject && db != null &&  db.IsTopLevelQueryFieldSupported(field.fieldName().GetText(), objectValue))
            {
                throw new Exception("Entry into db system not yet supported");
            }
            else
            {

                var resolvedValue =
                    ResolveFieldValue(objectType, objectValue, field.fieldName().GetText(), argumentValues);

                CompleteValue(fieldType, field, fields, resolvedValue, variableValues);
            }
        }


        Object ResolveFieldValue(__Type objectType, Object objectValue, String fieldName,
            Dictionary<string, Object> argumentValues)
        {
            var pi = objectType.dotNetType.GetTypeInfo().GetProperty(fieldName);
            return pi.GetValue(objectValue);
        }

        enum Context
        {
            Object,
            List
        }

        void CompleteValue(__Type fieldType, GraphQLParser.FieldContext field, List<GraphQLParser.FieldContext> fields,
            Object result,
            Dictionary<string, Object> variableValues, bool assertNotNull = false, Context context = Context.Object)
        {
            if (fieldType.kind == __TypeKind.NON_NULL)
            {
                var innerType = fieldType.ofType;
                CompleteValue(innerType, field, fields, result, variableValues, true);
            }
            else if (result == null)
            {
                if (assertNotNull)
                {
                    Error($"Null or empty value was found on non null fiell", field);
                }
            }
            else if (fieldType.kind == __TypeKind.LIST)
            {
                if (TypeCheck.IsEnumerableType(result.GetType()) == false)
                    Error($"Did not find list type for {field.fieldName().GetText()} - found {result.GetType().Name}",
                        field);
                var innerType = fieldType.ofType;

                output.PushArray(field.fieldName().GetText());
                foreach (var c in (IEnumerable) result)
                {
                    if (field.fieldName().GetText() == "types" && IsSchemaQuery && c is __Type &&
                        ((__Type) c).name.StartsWith("__"))
                        continue;
                    CompleteValue(innerType, field, fields, c, variableValues, false, Context.List);
                }
                output.Pop();
            }
            else if (fieldType.kind == __TypeKind.SCALAR || fieldType.kind == __TypeKind.ENUM)
            {
                CoerceDotNetValue(fieldType, field, result, context);
            }
            else if (fieldType.kind == __TypeKind.OBJECT || fieldType.kind == __TypeKind.UNION ||
                     fieldType.kind == __TypeKind.INTERFACE)
            {
                if (fieldType.kind == __TypeKind.OBJECT || fieldType.kind == __TypeKind.UNION)
                {
                    var objectType = fieldType;
                    var subSelectionSet = MergeSelectionSets(fields);

                    if (context == Context.Object)
                    {
                        output.PushObject(field.fieldName().GetText());
                    }
                    else if (context == Context.List)
                    {
                        output.PushObject();
                    }

                    ExecuteSelectionSet(subSelectionSet, objectType, result, variableValues);

                    output.Pop();
                }
                else
                {
                    Error($"Interface and Union not yet supported", field);
                }
            }
            else
            {
                Error($"Unexpected fieldType - {fieldType.kind}", field);
            }
        }

        ModifiableSelectionSet MergeSelectionSets(List<GraphQLParser.FieldContext> fields)
        {
            if (fields.Count() == 1 && fields.First().selectionSet() != null)
                return new ModifiableSelectionSet(fields.First().selectionSet());

            ModifiableSelectionSet ms = new ModifiableSelectionSet();
            foreach (var c in fields)
            {
                if (c.selectionSet() != null)
                {
                    foreach (var g in c.selectionSet().selection())
                    {
                        ms.selections.Add(g);
                    }
                }
            }
            return ms;
        }

        public Dictionary<string, Object> CoerceArgumentValues(__Type objectType, GraphQLParser.FieldContext field,
            Dictionary<string, Object> variableValues)
        {
            if (field.arguments() == null)
                return null;

            var coercedValues = new Dictionary<string, Object>();

            var argumentValues = field.arguments().argument();

            var fieldName = field.fieldName().GetText();

            var objectField = objectType.GetField((x) => x.name == fieldName);

            if (objectField == null)
                Error(
                    $"Could not find field with name - {fieldName} in objecttype with name: {objectType.name} and underlying type:{objectType.dotNetType.Name}",
                    field);

            foreach (var argumentDefinition in objectField.args)
            {
                var argumentName = argumentDefinition.name;
                var argumentType = argumentDefinition.type;
                var defaultValue = argumentDefinition.defaultValue;
                var value =
                    argumentValues.Where(x => x.NAME().GetText() == argumentName)
                        .Select(x => x.valueOrVariable())
                        .FirstOrDefault();
                if (value?.variable() != null)
                {
                    var variableName = value.variable().NAME().GetText();
                    Object variableValue = null;
                    if (variableValues.ContainsKey(variableName))
                        variableValue = variableValues[variableName];

                    if (variableValue != null)
                        coercedValues[argumentName] = variableValue;
                    else if (defaultValue != null)
                    {
                        coercedValues[argumentName] = defaultValue;
                    }
                    else if (argumentType.kind == __TypeKind.NON_NULL)
                        Error(
                            $"Required field '{argumentName}' refered by variable '{variableName}' is not present in document",
                            field);
                    else
                    {
                        continue;
                    }
                }
                else if (value?.value() == null)
                {
                    if (defaultValue != null)
                    {
                        coercedValues[argumentName] = defaultValue;
                    }
                    else if (argumentType.kind == __TypeKind.NON_NULL)
                        Error($"Required field '{argumentName}' is not present in document", field);
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    coercedValues[argumentName] = CoerceDocumentValue(argumentType, argumentName, value.value());
                }

            }
            return coercedValues;

        }

        void CoerceDotNetValue(__Type fieldType, GraphQLParser.FieldContext field, Object value,
            Context context = Context.Object)
        {
            try
            {
                if (fieldType.dotNetType == value.GetType())
                {
                    if (context == Context.List)
                    {
                        output.AddScalarValue(value);
                    }
                    else
                    {
                        output.AddScalarProperty(field.fieldName().GetText(), value);
                    }
                }
                else
                {
                    Error(
                        $"Error trying to coerce '{field.fieldName().GetText()}' of type '{value.GetType().Name}' to '{fieldType.kind}' with DotNetType: '{fieldType.dotNetType.Name}' ",
                        field);
                }

            }
            catch (Exception e)
            {
                Error(
                    $"Error - '{e.Message}' trying to coerce '{field.fieldName().GetText()}' of type '{value.GetType().Name}' to '{fieldType.kind}' with DotNetType: '{fieldType.dotNetType.Name}' ",
                    field);
            }
        }

        String Trim(String antlrString)
        {
            return antlrString.Substring(1, antlrString.Length - 2);
        }

        Object CoerceDocumentValue(__Type argumentType, String argumentName, GraphQLParser.ValueContext val)
        {
            try
            {
                if (val is GraphQLParser.StringValueContext)
                {
                    var typedVal = Trim(((GraphQLParser.StringValueContext) val).GetText());
                    if (argumentType.kind == __TypeKind.ENUM)
                    {
                        return Enum.Parse(argumentType.dotNetType, typedVal);
                    }
                    else if (argumentType.kind == __TypeKind.SCALAR)
                    {
                        if (TypeCheck.IsNumeric(argumentType.dotNetType))
                        {
                            Error($"Cannot convert string to numeric value", val);
                        }
                        else if (TypeCheck.IsString(argumentType.dotNetType))
                        {
                            return typedVal;
                        }
                        else if (TypeCheck.IsDateTime(argumentType.dotNetType))
                        {
                            return DateTime.Parse(typedVal);
                        }
                    }
                }
                else if (val is GraphQLParser.BooleanValueContext)
                {
                    var typedVal = (val.GetText() == "true") ? true : false;
                    if (argumentType.kind == __TypeKind.SCALAR)
                    {
                        if (TypeCheck.IsBoolean(argumentType.dotNetType))
                        {
                            return typedVal;
                        }
                        else if (TypeCheck.IsNumeric(argumentType.dotNetType))
                        {
                            if (typedVal)
                                return 1;
                            return 0;
                        }
                        else if (TypeCheck.IsString(argumentType.dotNetType))
                        {
                            if (typedVal)
                                return "true";
                            return false;
                        }
                    }
                }
                else if (val is GraphQLParser.NumberValueContext)
                {
                    var typedValue = Convert.ToDouble(val.GetText());

                    if (TypeCheck.IsNumeric(argumentType.dotNetType))
                    {
                        if (argumentType.dotNetType == typeof(int))
                            return (int) typedValue;
                        else if (argumentType.dotNetType == typeof(short))
                            return (short) typedValue;
                        else if (argumentType.dotNetType == typeof(uint))
                            return (uint) typedValue;
                        else if (argumentType.dotNetType == typeof(uint))
                            return (uint) typedValue;
                        else if (argumentType.dotNetType == typeof(float))
                            return (float) typedValue;
                        else
                            return typedValue;
                    }
                    else if (TypeCheck.IsString(argumentType.dotNetType))
                    {
                        return typedValue.ToString();
                    }
                    else if (TypeCheck.IsBoolean(argumentType.dotNetType))
                    {
                        if (typedValue == 0.0)
                            return false;
                        return true;
                    }
                }
                else if (val is GraphQLParser.ObjectValueContext)
                {
                    var jsonObj = GraphQlJson.FromJSonString(val.GetText(), argumentType.dotNetType, true);
                    return jsonObj;
                }
                else if (val is GraphQLParser.EnumValueContext)
                {
                    return Enum.Parse(argumentType.dotNetType, val.GetText());
                }
                else if (val is GraphQLParser.ArrayValueContext)
                {
                    List<Object> objects = new List<object>();
                    foreach (var c in ((GraphQLParser.ArrayValueContext) val).array().value())
                    {
                        objects.Add(CoerceDocumentValue(argumentType.ofType, argumentName, c));
                    }
                    return objects;
                }
                Error(
                    $"Encountered unexpected DotNetType when coercing value - {argumentName} - {argumentType.dotNetType.Name}",
                    val);
                return 0;
            }
            catch (Exception e)
            {
                Error(
                    $"Error - '{e.Message}' trying to coerce '{argumentName}' of type '{val.GetType().Name}' to '{argumentType.kind}' with DotNetType: '{argumentType.dotNetType.Name}' ",
                    val);
                return 0;
            }
        }
    }
}
