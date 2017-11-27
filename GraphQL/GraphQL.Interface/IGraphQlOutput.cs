using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Interface
{
    public interface IGraphQlOutput
    {
        void Push(Object data, string field, int pos = -1);
        void Push(String field, int pos = -1);
        void Pop();
        void PushList(List<Object> list, String field, int pos = -1);
        void PushList(string field, int pos = -1);

        void ProcessValue(String field, Object value, int pos = -1);
        void ProcessValue(Object value);

    }
}
