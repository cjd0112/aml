using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Interface;

namespace GraphQL.GraphQLSerializer
{
    public class GraphlLOutput : IGraphQlOutput
    {
        Stack<UnderlyingJson> stack = new Stack<UnderlyingJson>();

        StringBuilder builder = new StringBuilder();

        public void Push(Object o1, String fieldName, int pos = -1)
        {
            if (pos >= 100)
                throw new Exception($"currently only support up to 100 in field list");
            var obj = new UnderlyingUnorderedMap(stack.Count + 1);
            if (stack.Count == 0)
            {
                builder.Append(fieldName.ToString());
                builder.AppendLine(":{");
            }
            else
            {
                stack.Peek().AddValue(pos, fieldName.ToString(), obj);
            }
            stack.Push(obj);
        }

        public void Push(String field, int pos = -1)
        {
            PushOrderedObject(field);
        }

        void PushOrderedObject(String fieldName, int pos = -1)
        {
            if (pos >= 100)
                throw new Exception($"currently only support up to 100 in field list");

            var obj = new UnderlyingOrderedMap(stack.Count + 1);
            if (stack.Count == 0)
            {
                builder.Append("{ \"");
                builder.Append(fieldName);
                builder.AppendLine("\":");
            }
            else
            {
                stack.Peek().AddValue(pos, fieldName.ToString(), obj);
            }
            stack.Push(obj);
        }

        public void PushList(List<Object> obj1, String fieldName, int pos = -1)
        {
            PushList(fieldName.ToString(), pos);
        }

        public void PushList(string fieldName, int pos = -1)
        {
            if (pos >= 100)
                throw new Exception($"currently only support up to 100 in field list");

            var obj = new UnderlyingList(stack.Count + 1);
            if (stack.Count == 0)
            {
                builder.Append(fieldName);
                builder.Append(":");
            }
            else
            {
                stack.Peek().AddValue(pos, fieldName.ToString(), obj);
            }
            stack.Push(obj);
        }


        public void ProcessValue(String field, Object value, int pos = -1)
        {
            stack.Peek().AddValue(pos, field, value);
        }


        public void ProcessValue(Object value)
        {
            if (stack.Peek().GetType() != typeof(UnderlyingList))
                throw new Exception("Wrong type of object for operation - expected - UnderlyingList");
            stack.Peek().AddValue(0, "", value);

        }

        public void Pop()
        {
            stack.Peek().OnClosed();
            if (stack.Count == 1)
            {
                builder.Append(stack.Peek());
            }
            stack.Pop();
            if (stack.Count == 0)
            {
                builder.AppendLine();
                builder.AppendLine("}");
            }
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }

}
