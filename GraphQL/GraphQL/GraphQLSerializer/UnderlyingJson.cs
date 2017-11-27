using System;
using System.Collections;
using System.Text;

namespace GraphQL.GraphQLSerializer
{
    public abstract class UnderlyingJson
    {
        protected int PushCount;

        protected UnderlyingJson(int pushCount)
        {
            PushCount = pushCount;
        }


        protected void JSonSerialize(Object obj, StringBuilder b)
        {
            if (obj is String || obj is Enum || obj is DateTime)
            {
                b.Append('\"');
                b.Append(obj);
                b.Append("\"");
            }
            else if (obj is Boolean)
            {
                if ((Boolean)obj)
                    b.Append("true");
                else
                {
                    b.Append("false");
                }
            }
            else if (obj is IEnumerable)
            {
                b.Append("[");
                foreach (var c in (IEnumerable)obj)
                {
                    JSonSerialize(c, b);
                    b.Append(",");
                }
                b.Remove(b.Length - 1, 1);
                b.Append("]");
            }
            else
            {
                b.Append(obj.ToString());
            }
        }

        protected string StringRepresentation;

        public abstract void AddValue(int pos, String name, Object value);
        public abstract void OnClosed();

        public override string ToString()
        {
            return StringRepresentation;
        }
    }
}
