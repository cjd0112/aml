using System;
using System.Collections.Generic;
using System.Text;

namespace As.GraphQL.GraphQLSerializer
{
    public class UnderlyingOrderedMap : UnderlyingJson
    {
        List<Tuple<string, Object>> values = new List<Tuple<string, object>>();
        public UnderlyingOrderedMap(int pushCount) : base(pushCount)
        {

        }

        public override void AddValue(int pos, String fieldName, Object value)
        {
            values.Add(new Tuple<string, Object>(fieldName, value));
        }


        public override void OnClosed()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(' ');
            builder.AppendLine("{");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append(' ', PushCount + 1);

                builder.Append('\"');
                builder.Append(values[i].Item1);
                builder.Append("\":");

                JSonSerialize(values[i].Item2, builder);
                if (i != values.Count - 1)
                {
                    builder.AppendLine(",");
                }
                else
                {
                    builder.AppendLine();
                }

            }
            builder.Append(' ', PushCount);
            builder.Append("}");
            values.Clear();
            StringRepresentation = builder.ToString();
        }
    }
}
