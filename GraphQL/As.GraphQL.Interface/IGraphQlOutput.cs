using System;

namespace As.GraphQL.Interface
{
    public interface IGraphQlOutput
    {
        void AddScalarProperty(String field,Object value);
        void PushObject();
        void PushObject(string name);
        void PushArray(string name);
        void PushArray();
        void Pop();
        void AddScalarValue(object value);

        void AddException(IGraphQlException exception);

        Object GetRoot();

    }
}
