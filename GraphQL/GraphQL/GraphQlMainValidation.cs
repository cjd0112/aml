using System.Collections.Generic;
using System.Linq;
using GraphQL.Interface;
using GraphQLInterface.GraphQLType;

namespace GraphQL
{
    public class GraphQlMainValidation : GraphQlMainBase
    {
        public GraphQlMainValidation(IGraphQlDocument doc, IGraphQlSchema schema) : base(doc, schema)
        {
            OperationNameUniqueness();
            LoneAnonymousOperation();
            FragmentNameUniqueness();
            FieldSelectionMatchFragments();
            OperationSelectionMatch();
            FieldSelectionMerging();
        }


        void OperationNameUniqueness()
        {
            var namedOperations = GetOperations(x => x.NAME() != null).Select(x => x.GetText()).ToArray();
            if (namedOperations.Count() != namedOperations.Distinct().Count())
                Error("Operation names need to be unique ... ",documentContext);
        }

        void FragmentNameUniqueness()
        {
            var fragments = GetFragments(x => true).ToArray();
            if (fragments.Count() != fragments.Select((GraphQLParser.FragmentDefinitionContext n)=>n.fragmentName().GetText()).Distinct().Count())
                Error("Fragment names need to be unique ...", documentContext);
        }

        void LoneAnonymousOperation()
        {
            var anonymousOperations = GetOperations(x => x.NAME() == null).ToArray();
            if (anonymousOperations.Count() > 1 || (anonymousOperations.Count() == 1 && GetOperations(x=>x.NAME() != null).Any()) )
                Error("If anonymous operations are used - there can be at most one and can contain no 'named' operations",documentContext);
        }

        void OperationSelectionMatch()
        {
            foreach (var c in GetOperations(x => true))
            {
                var type = schema.__schema.queryType;
                if (c.selectionSet() != null)
                {
                    ValidateTypeAgainstSelectionSet(type.name,c.selectionSet());
                }
                else
                {
                    Error($"unexpected state - no selection set in operation",c);
                }
            }
        }

        void FieldSelectionMatchFragments()
        {
            var fragments = GetFragments(x => true);
            foreach (var f in fragments)
            {
                var typeName = f.typeCondition().typeName().NAME().GetText();

                ValidateTypeAgainstSelectionSet(typeName, f.selectionSet());

            }
        }

        void FieldSelectionMerging()
        {
            // TBD - deals with where fields have same names in selectionSets ... can 
            // be merged if they are unambiguous ... see specification
        }

        List<GraphQLParser.SelectionSetContext> validatedSelectionSets = new List<GraphQLParser.SelectionSetContext>();        

        void ValidateTypeAgainstSelectionSet(string typeName,GraphQLParser.SelectionSetContext selectionSet)
        {
            if (validatedSelectionSets.Contains(selectionSet) == false)
                validatedSelectionSets.Add(selectionSet);
            else
                return;

            var type = schema.GetType(typeName);
            if (type == null)
            {
                Error($"Type - {typeName} does not exist in schema", selectionSet);
            }


            foreach (var s in selectionSet.selection())
            {
                if (s.fragmentSpread() != null)
                {
                    var fragmentName = s.fragmentSpread().fragmentName().GetText();
                    var fragment = GetFragments(x => x.fragmentName().GetText() == fragmentName).FirstOrDefault();
                    if (fragment == null)
                        Error($"Cannot find fragment name in document - {fragmentName}",s.fragmentSpread());
                    else
                    {
                        ValidateTypeAgainstSelectionSet( fragment.typeCondition().GetText(),  fragment.selectionSet());
                    }
                }
                else if (s.inlineFragment() != null)
                {
                    // need to validate Interface here .... 
                    var inlineType = s.inlineFragment().typeCondition().GetText();
                    if (type.kind == __TypeKind.INTERFACE || type.kind == __TypeKind.UNION)
                    {
                        if (type.possibleTypes.Exists(x => x.name == inlineType) == false)
                        {
                            Error($"Type - {inlineType} is not a possible target for interface {type.name} - possibleTypes are {type.possibleTypes.Aggregate("", (x, y) => x + "," + y.name)}",s);
                        }
                    }
                    ValidateTypeAgainstSelectionSet(s.inlineFragment().typeCondition().GetText(),s.inlineFragment().selectionSet());
                }
                else
                {
                    var fieldName = s.field().fieldName().GetText();
                    var field = type.GetField((x)=>x.name == fieldName);
                    if (field == null)
                    {
                        if (fieldName == "__schema")
                        {
                            field = schema.GetType("__SchemaContainer").GetField((x)=>x.name == "__schema");
                        }
                        else
                        {
                            Error($"Field - {fieldName} does not exist in type - {type.name}", s);

                        }
                    }
                    if (s.field().arguments() != null)
                        ValidateArguments(field, s.field().arguments());

                    if (field.type.kind == __TypeKind.OBJECT || field.type.kind == __TypeKind.INTERFACE || field.type.kind == __TypeKind.UNION)
                    {
                        if (s.field().selectionSet() == null)
                            Error($"Received a field - {fieldName} - that is {field.type.kind} type - but query does not include selection set", s.field());

                        ValidateTypeAgainstSelectionSet(field.type.name, s.field().selectionSet());
                    }
                    else if (field.type.kind == __TypeKind.SCALAR)
                    {
                        if (s.field().selectionSet() != null)
                            Error($"Received a field - {fieldName} - that is SCALAR type - but query includes a selection set", s.field().selectionSet());
                    }
                    else if (field.type.kind == __TypeKind.ENUM)
                    {
                        if (s.field().selectionSet() != null)
                            Error($"Received a field - {fieldName} - that is ENUM type - but query includes a selection set", s.field().selectionSet());

                    }
                    else if (field.type.kind == __TypeKind.LIST)
                    {
                        if (field.type.ofType.kind != __TypeKind.SCALAR && field.type.ofType.kind != __TypeKind.ENUM)
                        {
                            if (s.field().selectionSet() == null)
                                Error(
                                    $"Received a field - {fieldName} - that is {field.type.kind} type with underlying type - {field.type.ofType.kind} - but query does not include selection set",
                                    s.field());

                            ValidateTypeAgainstSelectionSet(field.type.ofType.name, s.field().selectionSet());
                        }
                    }
                }
            }
        }

        void ValidateArguments(__Field field, GraphQLParser.ArgumentsContext arguments)
        {
            if (arguments.argument().Select(x=>x.NAME().GetText()).Distinct().Count() != arguments.argument().Count())
                Error($"argument names are not unique",arguments);

            foreach (var c in arguments.argument().Select(x => new {text= x.NAME().GetText(),ctx=x}))
            {
                var inputValue = field.GetInputValue(c.text);
                if (inputValue == null)
                    Error($"Received a named argument - {c.text} that is not expected for field {field.name}",c.ctx);

                var valueOrVariable = c.ctx.valueOrVariable();

                if (valueOrVariable.value() != null)
                {
                    ValidateArgumentType(valueOrVariable.value(), inputValue.type);
                }
            }

            foreach (var q in field.args)
            {
                if (q.type.kind == __TypeKind.NON_NULL)
                {
                    var argument = arguments.argument().FirstOrDefault(x => x.NAME().GetText() != q.name);
                    if (argument == null)
                    {
                        Error($"A required field - {q.name} is missing", arguments);
                        return;
                    }

                    if (argument.valueOrVariable().value().ToString() == "null")
                        Error($"A non-null field - {q.name} is called with null argument",arguments);
                }
            }
        }

        void ValidateArgumentType(GraphQLParser.ValueContext value, __Type input)
        {
            var checkType = input;
            // NON_NULL is indicator that type is necessary ofType represents underlying
            if (input.kind == __TypeKind.NON_NULL)
                checkType = input.ofType;

            if (value is GraphQLParser.StringValueContext)
            {
                if (checkType.kind != __TypeKind.SCALAR && (TypeCheck.IsString(checkType.dotNetType) == false && TypeCheck.IsEnum(checkType.dotNetType) == false))
                    Error($"String value in argument does not match input type - {checkType.name}", value);

            }
            else if (value is GraphQLParser.BooleanValueContext)
            {
                if(checkType.kind != __TypeKind.SCALAR && checkType.dotNetType != typeof(bool))
                    Error($"Boolean value in argument does not match checkType - {checkType.name}", value);
            }
            else if (value is GraphQLParser.NumberValueContext)
            {
                if (checkType.kind != __TypeKind.SCALAR && TypeCheck.IsNumeric(checkType.dotNetType) == false)
                    Error($"Numeric value in argument does not match checkType - {checkType.name}", value);

            }
            else if (value is GraphQLParser.ObjectValueContext)
            {
                if (checkType.kind != __TypeKind.INPUT_OBJECT)
                {
                    Error($"Object value in argument does not match checkType - {checkType.name}", value);

                }
            }
            else if (value is GraphQLParser.EnumValueContext)
            {
                if (checkType.kind != __TypeKind.ENUM)
                {
                    Error($"Enum value in argument does not match checkType - {checkType.name}", value);
                }
            }
            else if (value is GraphQLParser.ArrayValueContext)
            {
                if (checkType.kind != __TypeKind.LIST)
                {
                    Error($"Enum value in argument does not match checkType - {checkType.name}", value);
                }

                var lst = value as GraphQLParser.ArrayValueContext;

                foreach (var v in lst.array().value())
                {
                    ValidateArgumentType(v,checkType.ofType);
                }

            }
            else
            {
                Error($"Unexpected value type - {value.GetType()}",value);
            }
        }
    }
}

