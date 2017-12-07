using System;
using System.Text;

namespace As.GraphQL.GraphQLSerializer
{
    public class UnderlyingUnorderedMap : UnderlyingJson
    {
        public UnderlyingUnorderedMap(int pushCount) : base(pushCount)
        {

        }
        string[] names = new string[100];
        private Object[] values = new Object[100];
        private int maxPos = -1;

        public override void AddValue(int position, String field, Object value)
        {
            if (position == -1)
            {
                names[position] = field;
                values[position++] = value;

            }
            else
            {
                names[position] = field;
                values[position] = value;
            }
            maxPos = Math.Max(maxPos, position);
        }

        public override void OnClosed()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(' ');
            builder.AppendLine("{");
            for (int i = 0; i <= maxPos; i++)
            {
                if (names[i] != null)
                {
                    builder.Append(' ', PushCount + 1);
                    builder.Append('\"');
                    builder.Append(names[i]);
                    builder.Append('\"');
                    builder.Append(':');

                    JSonSerialize(values[i], builder);

                    if (i != maxPos)
                    {
                        builder.AppendLine(",");
                    }
                    else
                    {
                        builder.AppendLine();
                    }
                }
                names[i] = null;

            }
            builder.Append(' ', PushCount);
            builder.Append("}");
            names = null;
            values = null;
            StringRepresentation = builder.ToString();
        }
    }
}
