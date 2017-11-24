using System;

namespace GraphQLInterface
{
    public enum GraphQLAttributeTypeEnum
    {
        None,
        TreatAsInputObject,
        SupportSearchAllFields,
    }

    public class GraphQLAttribute : Attribute
    {

        public GraphQLAttributeTypeEnum AttributeType { get; set; }
        public GraphQLAttribute(GraphQLAttributeTypeEnum en)
        {
            AttributeType = en;
        }
        public Type RelationUnionType { get; set; }



        public GraphQLAttribute(Type type)
        {
            RelationUnionType = type;

        }
    }
}
